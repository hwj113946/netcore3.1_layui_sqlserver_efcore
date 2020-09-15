using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using netcore.Models;
using Helper;
using netcore_ef.Models;
using Microsoft.Data.SqlClient;
using NPOI.HSSF.Record.Chart;

namespace netcore.Controllers.WorkFlow
{
    public class WorkFlowController : Controller
    {
        private readonly netcore_databaseContext context;
        private readonly ILogger<WorkFlowController> logger;
        public WorkFlowController(netcore_databaseContext _context, ILogger<WorkFlowController> _logger)
        {
            context = _context;
            logger = _logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        //#region 获取审批通知
        //[HttpGet]
        //public async Task<IActionResult> GetApprNotice()
        //{
        //    string Msg = "";
        //    var ds = DbContextExtensions.RunProcCur(context.Database, "Get_WolkFlow_by_Person",
        //        new object[] { new SqlParameter("v_user_id",HttpContext.Session.GetInt32("user_id"))});
        //    Msg = ds.Tables[0].Rows.Count > 0
        //        ? "{\"code\":0,\"msg\":\"已查询到数据\",\"count\":" + ds.Tables[0].Rows.Count + ",\"data\":[" + JsonTools.DataTableToJson(ds.Tables[0]) + "]}"
        //        : "{\"code\":0,\"msg\":\"未查询到数据\",\"count\":0,\"data\":[]}";
        //    return Content(Msg);
        //}
        //#endregion

        #region 创建审批流
        [HttpPost]
        public async Task<string> Create_WorkFlow(int ApprTypeId, int DocId, int DepId)
        {
            var n = 0;
            var v_Sf_Cxsc = ""; //是否重新生成
            var v_Sf_Tpi = ""; //是否有单独授权表
            var v_Appr_Flow_Id = 0;
            var w_n = 0; //审批流顺序
            var r_n = 0; //返回审批流顺序
            var v_Curr_Node_Code = "";
            var v_Curr_Node_Type = "";
            var v_Upper_Node = "";
            var v_Next_Line_Code = "";
            var v_Next_Node_Code = "";
            var v_Next_Node_Name = "";
            var v_Next_Node_Type = "";
            var v_Upper_Rect = 0;
            var v_Curr_Rect = 0;
            var v_Curr_Rect_Name = "";
            var p_Lp_Tj = "";
            var Sql_Imd = "";
            var t_n = 0;
            if (ApprTypeId == -99)
            {
                return "Y";
            }
            using (var tran=context.Database.BeginTransaction())
            {
                //获取连接线
                var lines_list = await context.AppWorkflowApprLines.Where(u => u.ApprFlowId == v_Curr_Rect && u.From == v_Curr_Node_Code).ToListAsync();
                //获取连接线代码及连接线条件
                var LineCodeAndProList = await GetLineCodeAndProList(v_Curr_Rect, v_Curr_Node_Code, null);
                t_n = await context.AppWorkflowApprTranTemps.CountAsync(u => u.ApprFlowId == ApprTypeId && u.CurrNodeType != "rect" && u.DocId == DocId);
                //获取审批流类型里面的表信息：table_name表名，table_pk_name主键名称，create_flow_again是否重新生成流程
                var TableInfo = await context.AppWorkflowApprTypes.SingleOrDefaultAsync(u => u.ApprTypeId == ApprTypeId);
                var listcount = await (from t in context.AppWorkflowApprTypes
                                       from f in context.AppWorkflowApprFlows
                                       where
                                         t.Status == "有效" &&
                                         t.ApprTypeId == f.ApprTypeId &&
                                         t.ApprTypeId == ApprTypeId
                                       select new
                                       {
                                           t.ApprTypeId,
                                           f.ApprFlowId
                                       }).ToListAsync();
                n = listcount.Count;
                if (n == 1)
                {
                    v_Appr_Flow_Id = listcount[0].ApprFlowId;
                }
                else
                {
                    await tran.RollbackAsync();
                    return "未定义审批流，请联系管理员";
                }
                //是否重新生成
                if (TableInfo.CreateFlowAgain=="是"|| t_n==0)
                {
                    var temp = await context.AppWorkflowApprTranTemps.Where(u => u.ApprTypeId == ApprTypeId && u.DocId == DocId).ToListAsync();
                    if (temp.Count>0)
                    {
                        context.AppWorkflowApprTranTemps.RemoveRange(temp);
                        context.SaveChanges();
                    }
                    var temp1=await context.AppWorkflowApprTranTemps.Where(u => u.ApprFlowId == v_Appr_Flow_Id && u.DocId == DocId).ToListAsync();
                    if (temp1.Count > 0)
                    {
                        context.AppWorkflowApprTranTemps.RemoveRange(temp1);
                        context.SaveChanges();
                    }
                    w_n = 1;
                    v_Curr_Rect = v_Appr_Flow_Id;

                    if (!string.IsNullOrEmpty(v_Curr_Rect.ToString()??""))
                    {
                        v_Curr_Node_Code = "";
                        while (true)
                        {
                            if (v_Curr_Node_Code == "" || string.IsNullOrEmpty(v_Curr_Node_Code))
                            {
                                n = await context.AppWorkflowApprNodes.CountAsync(u => u.ApprFlowId == v_Curr_Rect && u.Type == "start");
                                if (n == 0)
                                {
                                    await tran.RollbackAsync();
                                    return "未定义审批流（没有开始节点），请联系管理员";
                                }
                                else
                                {
                                    var single_node = await context.AppWorkflowApprNodes.SingleOrDefaultAsync(u=>u.ApprFlowId==v_Curr_Rect&&u.Type=="start");
                                    v_Curr_Node_Code = single_node.NodeCode;
                                    v_Curr_Node_Type = single_node.Type;
                                    if (v_Curr_Rect==v_Appr_Flow_Id)
                                    {
                                        var node_list=await context.AppWorkflowApprNodes.Where(u => u.ApprFlowId == v_Curr_Rect && u.Type == "start").ToListAsync();
                                        for (int i = 0; i < node_list.Count; i++)
                                        {
                                            context.AppWorkflowApprTranTemps.Add(new AppWorkflowApprTranTemp()
                                            {
                                                WN = w_n,
                                                ApprTypeId = ApprTypeId,
                                                ApprFlowId = v_Appr_Flow_Id,
                                                DocId = DocId,
                                                CurrNodeCode = node_list[i].NodeCode,
                                                CurrNoteName = node_list[i].NodeName,
                                                CurrNodeType = node_list[i].Type,
                                                CurrRect = node_list[i].ApprFlowId,
                                                CreationDate = DateTime.Now
                                            });
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }

                            n = await context.AppWorkflowApprLines.CountAsync(u=>u.ApprFlowId==v_Curr_Rect&&u.From==v_Curr_Node_Code);
                            if (n==0)
                            {
                                await tran.RollbackAsync();
                                return "审批流设置错误：无法找到下一审批节点，请联系管理员";
                            }
                            if (n==1)
                            {
                                var single_line=await context.AppWorkflowApprLines.SingleOrDefaultAsync(u => u.ApprFlowId == v_Curr_Rect && u.From == v_Curr_Node_Code);
                                v_Next_Node_Code = single_line.To;
                            }
                            else
                            {
                                for (int i = 0; i < lines_list.Count; i++)
                                {
                                    p_Lp_Tj = "";
                                    LineCodeAndProList =await GetLineCodeAndProList(v_Curr_Rect, v_Curr_Node_Code, lines_list[i].LineCode);
                                    for (int j = 0; j < LineCodeAndProList.Count; j++)
                                    {
                                        p_Lp_Tj += " and ( "+LineCodeAndProList[j].Sql+" )";
                                    }
                                    Sql_Imd = "select count(*) from " + TableInfo.TableName + " doc where " + TableInfo.TablePkName + " = '" + DocId + "' " + p_Lp_Tj;
                                    n = await context.Database.ExecuteSqlRawAsync(Sql_Imd);
                                    if (n==1)
                                    {
                                        v_Next_Line_Code = lines_list[i].LineCode;
                                        var single_line1 = await context.AppWorkflowApprLines.SingleOrDefaultAsync(u=>u.ApprFlowId==v_Curr_Rect&&u.From==v_Curr_Node_Code&&u.LineCode==v_Next_Line_Code);
                                        v_Next_Node_Code = single_line1.To;
                                    }
                                }
                                if (string.IsNullOrEmpty(v_Next_Node_Code))
                                {
                                    await tran.RollbackAsync();
                                    return "审批流设置错误：无法找到下一审批节点，请联系管理员";
                                }
                            }
                            var single2 = await (from ns in context.AppWorkflowApprNodes
                                                 where
                                                   ns.ApprFlowId == v_Curr_Rect &&
                                                   ns.NodeCode == v_Next_Node_Code
                                                 select new
                                                 {
                                                     ns.Type,
                                                     NextNodeName = ns.Type == "start" ? "开始" : ns.Type == "end" ? "结束" : ns.NodeName
                                                 }).SingleOrDefaultAsync();
                            v_Next_Node_Type = single2.Type;
                            v_Next_Node_Name = single2.NextNodeName;
                            if (v_Next_Node_Type=="rect")
                            {
                                v_Upper_Node = v_Next_Node_Code;
                                v_Upper_Rect = v_Curr_Rect;
                                var single3 = await (from ns in context.AppWorkflowApprNodes
                                                     from p in context.AppWorkflowNodePropeties
                                                     from f in context.AppWorkflowApprFlows
                                                     where
                                                       ns.ApprFlowId == p.ApprFlowId &&
                                                       ns.NodeCode == p.NodeCode &&
                                                       p.Rect == f.ApprFlowId.ToString() &&
                                                       ns.ApprFlowId == v_Curr_Rect &&
                                                       ns.NodeCode == v_Next_Node_Code
                                                     select new
                                                     {
                                                         p.Rect,
                                                         f.ApprFlowName
                                                     }).SingleOrDefaultAsync();
                                v_Curr_Rect = int.Parse(single3.Rect);
                                v_Curr_Rect_Name = single3.ApprFlowName;
                            }
                            w_n++;
                            v_Curr_Node_Code = v_Next_Node_Code;
                            context.AppWorkflowApprTranTemps.Add(new AppWorkflowApprTranTemp()
                            {
                                WN = w_n,
                                ApprTypeId = ApprTypeId,
                                ApprFlowId = v_Appr_Flow_Id,
                                DocId = DocId,
                                CurrNodeCode = v_Next_Node_Code,
                                CurrNoteName = v_Next_Node_Name,
                                CurrNodeType = v_Next_Node_Type,
                                CurrRect = v_Curr_Rect,
                                CreationDate = DateTime.Now
                            });
                            context.SaveChanges();
                            if (v_Next_Node_Type=="end" && v_Appr_Flow_Id!=v_Curr_Rect)
                            {
                                await tran.RollbackAsync();
                                return "审批流错误，请联系管理员";
                            }
                            if (v_Next_Node_Type == "end" && v_Appr_Flow_Id == v_Curr_Rect)
                            {
                                await tran.CommitAsync();
                                return "Y";
                            }
                        }
                    }
                }
            }
            return "";
        }
        #endregion

        #region 获取连接线代码及连接线条件
        private async Task<List<LineCodeAndPro>> GetLineCodeAndProList(int ApprFlowId, string From, string LineCode)
        {
            var LineCodeAndProList = await (from l in context.AppWorkflowApprLines
                                            join lp in context.AppWorkflowLineProperties
                                            on l.ApprFlowId equals lp.ApprFlowId
                                            where l.LineCode.Equals(lp.LineCode)
                                            where l.ApprFlowId.Equals(ApprFlowId)
                                            where l.From.Equals(From)
                                            where l.LineCode.Equals(LineCode)
                                            select new LineCodeAndPro
                                            {
                                                LineCode = l.LineCode,
                                                Sql = lp.Sql
                                            }).ToListAsync();
            return LineCodeAndProList;
        } 
        #endregion
    }
    public class LineCodeAndPro
    {
        public string LineCode { get; set; }
        public string Sql { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore.Models.Excel.ExcelTitle
{
    public class ExcelTitle
    {
        public static string[] Corp = {"主键ID（忽略）" ,"公司代码", "公司名称", "传真", "邮政编码", "Email", "公司联系人姓名", "联系人手机号", "联系人身份证", "法人姓名", "法人手机号","法人身份证","公司地址","登记纳税号","备注","状态","最后修改数据时间","最后修改数据人主键","创建数据时间","创建人主键" };
        public static string[] AppCorpBank = {"所属公司", "开户行省份", "开户行城市", "开户银行", "银行帐号", "行号", "状态","备注" };
        public static string[] AppDept = { "所属公司","部门代码","部门名称","备注","状态"};
    }
}

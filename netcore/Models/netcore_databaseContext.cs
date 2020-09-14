using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace netcore.Models
{
    public partial class netcore_databaseContext : DbContext
    {
        public netcore_databaseContext()
        {
        }

        public netcore_databaseContext(DbContextOptions<netcore_databaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppButton> AppButtons { get; set; }
        public virtual DbSet<AppCorp> AppCorps { get; set; }
        public virtual DbSet<AppCorpBank> AppCorpBanks { get; set; }
        public virtual DbSet<AppDept> AppDepts { get; set; }
        public virtual DbSet<AppFixvalue> AppFixvalues { get; set; }
        public virtual DbSet<AppFixvalueType> AppFixvalueTypes { get; set; }
        public virtual DbSet<AppMenu> AppMenus { get; set; }
        public virtual DbSet<AppMenuButton> AppMenuButtons { get; set; }
        public virtual DbSet<AppMessageBoard> AppMessageBoards { get; set; }
        public virtual DbSet<AppNews> AppNews { get; set; }
        public virtual DbSet<AppNewsType> AppNewsTypes { get; set; }
        public virtual DbSet<AppPost> AppPosts { get; set; }
        public virtual DbSet<AppRole> AppRoles { get; set; }
        public virtual DbSet<AppRoleMenu> AppRoleMenus { get; set; }
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<AppUserRole> AppUserRoles { get; set; }
        public virtual DbSet<AppWorkflowAppr> AppWorkflowApprs { get; set; }
        public virtual DbSet<AppWorkflowApprFlow> AppWorkflowApprFlows { get; set; }
        public virtual DbSet<AppWorkflowApprLine> AppWorkflowApprLines { get; set; }
        public virtual DbSet<AppWorkflowApprNode> AppWorkflowApprNodes { get; set; }
        public virtual DbSet<AppWorkflowApprTran> AppWorkflowApprTrans { get; set; }
        public virtual DbSet<AppWorkflowApprTranTemp> AppWorkflowApprTranTemps { get; set; }
        public virtual DbSet<AppWorkflowApprType> AppWorkflowApprTypes { get; set; }
        public virtual DbSet<AppWorkflowLineProperty> AppWorkflowLineProperties { get; set; }
        public virtual DbSet<AppWorkflowNodePropety> AppWorkflowNodePropeties { get; set; }
        public virtual DbSet<AppWorkflowReadTran> AppWorkflowReadTrans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=localhost;uid=sa;pwd=hwj;database=netcore_database;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppButton>(entity =>
            {
                entity.HasKey(x => x.ButtonId)
                    .HasName("PK_app_buttion");

                entity.ToTable("app_button");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ButtonId).HasColumnName("button_id");

                entity.Property(e => e.ButtonColor)
                    .HasMaxLength(100)
                    .HasColumnName("button_color");

                entity.Property(e => e.ButtonElementId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("button_element_id")
                    .HasComment("元素id（如：input的id）");

                entity.Property(e => e.ButtonEvent)
                    .HasMaxLength(100)
                    .HasColumnName("button_event");

                entity.Property(e => e.ButtonIcon)
                    .HasMaxLength(100)
                    .HasColumnName("button_icon");

                entity.Property(e => e.ButtonName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("button_name");

                entity.Property(e => e.ButtonSort).HasColumnName("button_sort");
            });

            modelBuilder.Entity<AppCorp>(entity =>
            {
                entity.HasKey(x => x.CorpId);

                entity.ToTable("app_corp");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.CorpId).HasColumnName("corp_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasColumnName("address")
                    .HasComment("公司地址");

                entity.Property(e => e.ContractPersonIdentity)
                    .HasMaxLength(30)
                    .HasColumnName("contract_person_identity")
                    .HasComment("联系人身份证");

                entity.Property(e => e.ContractPersonName)
                    .HasMaxLength(50)
                    .HasColumnName("contract_person_name")
                    .HasComment("联系人姓名");

                entity.Property(e => e.ContractPersonPhone)
                    .HasMaxLength(11)
                    .HasColumnName("contract_person_phone")
                    .HasComment("联系人电话");

                entity.Property(e => e.CorpCode)
                    .HasMaxLength(50)
                    .HasColumnName("corp_code")
                    .HasComment("公司代码");

                entity.Property(e => e.CorpName)
                    .HasMaxLength(300)
                    .HasColumnName("corp_name")
                    .HasComment("公司名称");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Fax)
                    .HasMaxLength(50)
                    .HasColumnName("fax");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.LawPersonIdentity)
                    .HasMaxLength(30)
                    .HasColumnName("law_person_identity")
                    .HasComment("法人代表身份证号码");

                entity.Property(e => e.LawPersonName)
                    .HasMaxLength(50)
                    .HasColumnName("law_person_name")
                    .HasComment("法人代表姓名");

                entity.Property(e => e.LawPersonPhone)
                    .HasMaxLength(11)
                    .HasColumnName("law_person_phone")
                    .HasComment("法人代表联系电话");

                entity.Property(e => e.Note)
                    .HasMaxLength(3000)
                    .HasColumnName("note");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("编辑；有效；失效");

                entity.Property(e => e.TaxRqNumber)
                    .HasMaxLength(50)
                    .HasColumnName("tax_rq_number")
                    .HasComment("纳税登记号");

                entity.Property(e => e.Zip)
                    .HasMaxLength(50)
                    .HasColumnName("zip");
            });

            modelBuilder.Entity<AppCorpBank>(entity =>
            {
                entity.HasKey(x => x.CorpBankId);

                entity.ToTable("app_corp_bank");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.CorpBankId).HasColumnName("corp_bank_id");

                entity.Property(e => e.BankAccount)
                    .HasMaxLength(80)
                    .HasColumnName("bank_account")
                    .HasComment("银行帐号");

                entity.Property(e => e.BankCity)
                    .HasMaxLength(50)
                    .HasColumnName("bank_city")
                    .HasComment("开户行城市");

                entity.Property(e => e.BankName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("bank_name")
                    .HasComment("开户银行");

                entity.Property(e => e.BankNo)
                    .HasMaxLength(50)
                    .HasColumnName("bank_no")
                    .HasComment("行号");

                entity.Property(e => e.BankProvince)
                    .HasMaxLength(50)
                    .HasColumnName("bank_province")
                    .HasComment("开户行省份");

                entity.Property(e => e.CorpId)
                    .HasColumnName("corp_id")
                    .HasComment("公司id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.Note)
                    .HasMaxLength(3000)
                    .HasColumnName("note");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("编辑；有效；失效");
            });

            modelBuilder.Entity<AppDept>(entity =>
            {
                entity.HasKey(x => x.DeptId);

                entity.ToTable("app_dept");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.DeptId).HasColumnName("dept_id");

                entity.Property(e => e.CorpId).HasColumnName("corp_id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.DeptCode)
                    .HasMaxLength(50)
                    .HasColumnName("dept_code");

                entity.Property(e => e.DeptName)
                    .HasMaxLength(100)
                    .HasColumnName("dept_name");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.Note)
                    .HasMaxLength(3000)
                    .HasColumnName("note");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("编辑；有效；失效");
            });

            modelBuilder.Entity<AppFixvalue>(entity =>
            {
                entity.HasKey(x => x.FixvalueId);

                entity.ToTable("app_fixvalue");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.FixvalueId).HasColumnName("fixvalue_id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.FixvalueCode)
                    .HasMaxLength(3000)
                    .HasColumnName("fixvalue_code");

                entity.Property(e => e.FixvalueName)
                    .HasMaxLength(3000)
                    .HasColumnName("fixvalue_name");

                entity.Property(e => e.FixvalueTypeId).HasColumnName("fixvalue_type_id");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.Note)
                    .HasMaxLength(300)
                    .HasColumnName("note");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<AppFixvalueType>(entity =>
            {
                entity.HasKey(x => x.FixvalueTypeId);

                entity.ToTable("app_fixvalue_type");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.FixvalueTypeId).HasColumnName("fixvalue_type_id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.FixvalueTypeCode)
                    .HasMaxLength(50)
                    .HasColumnName("fixvalue_type_code");

                entity.Property(e => e.FixvalueTypeName)
                    .HasMaxLength(100)
                    .HasColumnName("fixvalue_type_name");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.Note)
                    .HasMaxLength(300)
                    .HasColumnName("note");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<AppMenu>(entity =>
            {
                entity.HasKey(x => x.MenuId);

                entity.ToTable("app_menu");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.MenuIcon)
                    .HasMaxLength(300)
                    .HasColumnName("menu_icon");

                entity.Property(e => e.MenuName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("menu_name");

                entity.Property(e => e.MenuSort).HasColumnName("menu_sort");

                entity.Property(e => e.MenuType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("menu_type");

                entity.Property(e => e.MenuUrl)
                    .HasMaxLength(300)
                    .HasColumnName("menu_url");

                entity.Property(e => e.ParentMenuId).HasColumnName("parent_menu_id");
            });

            modelBuilder.Entity<AppMenuButton>(entity =>
            {
                entity.HasKey(x => x.MenuButtonId);

                entity.ToTable("app_menu_button");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.MenuButtonId).HasColumnName("menu_button_id");

                entity.Property(e => e.ButtonId).HasColumnName("button_id");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");
            });

            modelBuilder.Entity<AppMessageBoard>(entity =>
            {
                entity.HasKey(x => x.MessageId);

                entity.ToTable("app_message_board");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.MessageId).HasColumnName("message_id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.MessageContent)
                    .HasColumnName("message_content")
                    .HasComment("信息内容");

                entity.Property(e => e.MessageType)
                    .HasMaxLength(50)
                    .HasColumnName("message_type")
                    .HasComment("来源类型");

                entity.Property(e => e.SourceId)
                    .HasColumnName("source_id")
                    .HasComment("来源id");
            });

            modelBuilder.Entity<AppNews>(entity =>
            {
                entity.HasKey(x => x.NewsId);

                entity.ToTable("app_news");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.NewsId).HasColumnName("news_id");

                entity.Property(e => e.BrowseNumber).HasColumnName("browse_number");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.NewsAuthor)
                    .HasMaxLength(50)
                    .HasColumnName("news_author");

                entity.Property(e => e.NewsContent).HasColumnName("news_content");

                entity.Property(e => e.NewsCoverImageUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("news_cover_image_url");

                entity.Property(e => e.NewsReleaseTime)
                    .HasColumnType("datetime")
                    .HasColumnName("news_release_time")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.NewsTitle)
                    .HasMaxLength(300)
                    .HasColumnName("news_title");

                entity.Property(e => e.NewsTypeId).HasColumnName("news_type_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<AppNewsType>(entity =>
            {
                entity.HasKey(x => x.NewsTypeId);

                entity.ToTable("app_news_type");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.NewsTypeId).HasColumnName("news_type_id");

                entity.Property(e => e.NewsTypeName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("news_type_name");
            });

            modelBuilder.Entity<AppPost>(entity =>
            {
                entity.HasKey(x => x.PostId);

                entity.ToTable("app_post");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.PostId).HasColumnName("post_id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.DeptId).HasColumnName("dept_id");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.Note)
                    .HasMaxLength(3000)
                    .HasColumnName("note");

                entity.Property(e => e.ParentPostId).HasColumnName("parent_post_id");

                entity.Property(e => e.PostCode)
                    .HasMaxLength(50)
                    .HasColumnName("post_code");

                entity.Property(e => e.PostName)
                    .HasMaxLength(100)
                    .HasColumnName("post_name");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("编辑；有效；失效");
            });

            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.HasKey(x => x.RoleId);

                entity.ToTable("app_role");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<AppRoleMenu>(entity =>
            {
                entity.HasKey(x => x.RoleMenuId);

                entity.ToTable("app_role_menu");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.RoleMenuId).HasColumnName("role_menu_id");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(x => x.UserId);

                entity.ToTable("app_user");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(300)
                    .HasColumnName("address");

                entity.Property(e => e.CorpId).HasColumnName("corp_id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.DeptId).HasColumnName("dept_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.IdCardNumber)
                    .HasMaxLength(50)
                    .HasColumnName("id_card_number");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.ModifyPasswordDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modify_password_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");

                entity.Property(e => e.PostId).HasColumnName("post_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.UserCode)
                    .HasMaxLength(50)
                    .HasColumnName("user_code");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("user_name");
            });

            modelBuilder.Entity<AppUserRole>(entity =>
            {
                entity.HasKey(x => x.UserRoleId);

                entity.ToTable("app_user_role");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<AppWorkflowAppr>(entity =>
            {
                entity.HasKey(x => x.ApprId);

                entity.ToTable("app_workflow_appr");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ApprId).HasColumnName("appr_id");

                entity.Property(e => e.ApplCorpId)
                    .HasColumnName("appl_corp_id")
                    .HasComment("发布人公司");

                entity.Property(e => e.ApplDate)
                    .HasColumnType("datetime")
                    .HasColumnName("appl_date")
                    .HasComment("发布时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.ApplDeptId)
                    .HasColumnName("appl_dept_id")
                    .HasComment("发布人部门");

                entity.Property(e => e.ApplPostId)
                    .HasColumnName("appl_post_id")
                    .HasComment("发布人岗位");

                entity.Property(e => e.ApplUserId)
                    .HasColumnName("appl_user_id")
                    .HasComment("发布人");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("审批流id");

                entity.Property(e => e.ApprNodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("appr_node_code")
                    .HasComment("审批人的节点代码");

                entity.Property(e => e.ApprNodeType)
                    .HasMaxLength(50)
                    .HasColumnName("appr_node_type")
                    .HasComment("审批人的节点类型");

                entity.Property(e => e.ConfirmNodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("confirm_node_code")
                    .HasComment("提交人的节点代码");

                entity.Property(e => e.ConfirmNodeType)
                    .HasMaxLength(50)
                    .HasColumnName("confirm_node_type")
                    .HasComment("提交人的节点类型");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.CurrApprNote)
                    .HasMaxLength(3000)
                    .HasColumnName("curr_appr_note")
                    .HasComment("当前审批人意见");

                entity.Property(e => e.CurrApprUserId)
                    .HasColumnName("curr_appr_user_id")
                    .HasComment("当前审批人id");

                entity.Property(e => e.CurrFlowId)
                    .HasColumnName("curr_flow_id")
                    .HasComment("当前审批节点的flowid");

                entity.Property(e => e.DocId)
                    .HasColumnName("doc_id")
                    .HasComment("数据id");

                entity.Property(e => e.DocNote)
                    .HasMaxLength(3000)
                    .HasColumnName("doc_note")
                    .HasComment("说明");

                entity.Property(e => e.DocTitle)
                    .HasMaxLength(1000)
                    .HasColumnName("doc_title")
                    .HasComment("标题");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.PageViewUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("page_view_url")
                    .HasComment("查看界面地址");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("状态：编辑；审批中；审批通过；退回；");

                entity.Property(e => e.UpperFlowId)
                    .HasColumnName("upper_flow_id")
                    .HasComment("上一审批节点的flowid");

                entity.Property(e => e.UpperNodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("upper_node_code")
                    .HasComment("上一审批流节点的代码");
            });

            modelBuilder.Entity<AppWorkflowApprFlow>(entity =>
            {
                entity.HasKey(x => x.ApprFlowId);

                entity.ToTable("app_workflow_appr_flow");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ApprFlowId).HasColumnName("appr_flow_id");

                entity.Property(e => e.ApprFlowName)
                    .HasMaxLength(300)
                    .HasColumnName("appr_flow_name")
                    .HasComment("审批流名称");

                entity.Property(e => e.ApprTypeId)
                    .HasColumnName("appr_type_id")
                    .HasComment("审批流类型id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.Note)
                    .HasMaxLength(300)
                    .HasColumnName("note")
                    .HasComment("说明");
            });

            modelBuilder.Entity<AppWorkflowApprLine>(entity =>
            {
                entity.ToTable("app_workflow_appr_lines");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("审批流id");

                entity.Property(e => e.From)
                    .HasMaxLength(300)
                    .HasColumnName("from")
                    .HasComment("来源节点代码");

                entity.Property(e => e.LineCode)
                    .HasMaxLength(300)
                    .HasColumnName("line_code")
                    .HasComment("连接线代码");

                entity.Property(e => e.LineName)
                    .HasMaxLength(300)
                    .HasColumnName("line_name")
                    .HasComment("连接线名称");

                entity.Property(e => e.Num)
                    .HasColumnName("num")
                    .HasComment("序列:排序");

                entity.Property(e => e.To)
                    .HasMaxLength(300)
                    .HasColumnName("to")
                    .HasComment("指向节点代码");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .HasColumnName("type")
                    .HasComment("类型：line");
            });

            modelBuilder.Entity<AppWorkflowApprNode>(entity =>
            {
                entity.ToTable("app_workflow_appr_nodes");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("工作流id");

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.Left).HasColumnName("left");

                entity.Property(e => e.NodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("node_code")
                    .HasComment("节点代码");

                entity.Property(e => e.NodeName)
                    .HasMaxLength(300)
                    .HasColumnName("node_name")
                    .HasComment("节点名称");

                entity.Property(e => e.Num).HasColumnName("num");

                entity.Property(e => e.Top).HasColumnName("top");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type")
                    .HasComment("节点类型");

                entity.Property(e => e.Width).HasColumnName("width");
            });

            modelBuilder.Entity<AppWorkflowApprTran>(entity =>
            {
                entity.HasKey(x => x.ApprTranId);

                entity.ToTable("app_workflow_appr_tran");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ApprTranId).HasColumnName("appr_tran_id");

                entity.Property(e => e.ApprCorpId)
                    .HasColumnName("appr_corp_id")
                    .HasComment("审批人公司id");

                entity.Property(e => e.ApprDate)
                    .HasColumnType("datetime")
                    .HasColumnName("appr_date")
                    .HasComment("审批时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.ApprDeptId)
                    .HasColumnName("appr_dept_id")
                    .HasComment("审批人部门id");

                entity.Property(e => e.ApprId)
                    .HasColumnName("appr_id")
                    .HasComment("审批id");

                entity.Property(e => e.ApprNote)
                    .HasColumnName("appr_note")
                    .HasComment("审批意见");

                entity.Property(e => e.ApprPostId)
                    .HasColumnName("appr_post_id")
                    .HasComment("审批人岗位id");

                entity.Property(e => e.ApprUserId)
                    .HasColumnName("appr_user_id")
                    .HasComment("审批人id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.CurrFlowId)
                    .HasColumnName("curr_flow_id")
                    .HasComment("当前flowid");

                entity.Property(e => e.CurrNote)
                    .HasMaxLength(300)
                    .HasColumnName("curr_note")
                    .HasComment("当前节点代码");

                entity.Property(e => e.CurrType)
                    .HasMaxLength(50)
                    .HasColumnName("curr_type")
                    .HasComment("当前节点类型");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.NextCode)
                    .HasMaxLength(300)
                    .HasColumnName("next_code")
                    .HasComment("下一节点代码");

                entity.Property(e => e.NextType)
                    .HasMaxLength(50)
                    .HasColumnName("next_type")
                    .HasComment("下一节点类型");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("状态：待审批；同意；收回；不同意（退回）；收回");
            });

            modelBuilder.Entity<AppWorkflowApprTranTemp>(entity =>
            {
                entity.ToTable("app_workflow_appr_tran_temp");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("审批流id");

                entity.Property(e => e.ApprTypeId)
                    .HasColumnName("appr_type_id")
                    .HasComment("审批类型id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CurrNodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("curr_node_code")
                    .HasComment("当前节点代码");

                entity.Property(e => e.CurrNodeType)
                    .HasMaxLength(50)
                    .HasColumnName("curr_node_type")
                    .HasComment("当前节点类型");

                entity.Property(e => e.CurrNoteName)
                    .HasMaxLength(300)
                    .HasColumnName("curr_note_name");

                entity.Property(e => e.CurrRect).HasColumnName("curr_rect");

                entity.Property(e => e.DocId)
                    .HasColumnName("doc_id")
                    .HasComment("数据id");

                entity.Property(e => e.NextNodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("next_node_code")
                    .HasComment("下一节点代码");

                entity.Property(e => e.NextNodeType)
                    .HasMaxLength(50)
                    .HasColumnName("next_node_type")
                    .HasComment("下一节点类型");

                entity.Property(e => e.UpperNode)
                    .HasMaxLength(300)
                    .HasColumnName("upper_node");

                entity.Property(e => e.UpperRect).HasColumnName("upper_rect");

                entity.Property(e => e.WN)
                    .HasColumnName("w_n")
                    .HasComment("排序");
            });

            modelBuilder.Entity<AppWorkflowApprType>(entity =>
            {
                entity.HasKey(x => x.ApprTypeId);

                entity.ToTable("app_workflow_appr_type");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ApprTypeId).HasColumnName("appr_type_id");

                entity.Property(e => e.ApprCancelStatus)
                    .HasMaxLength(50)
                    .HasColumnName("appr_cancel_status")
                    .HasComment("取消状态");

                entity.Property(e => e.ApprEndStatus)
                    .HasMaxLength(50)
                    .HasColumnName("appr_end_status")
                    .HasComment("结束状态");

                entity.Property(e => e.ApprStartStatus)
                    .HasMaxLength(50)
                    .HasColumnName("appr_start_status")
                    .HasComment("开始状态");

                entity.Property(e => e.ApprTypeCode)
                    .HasMaxLength(100)
                    .HasColumnName("appr_type_code");

                entity.Property(e => e.ApprTypeName)
                    .HasMaxLength(300)
                    .HasColumnName("appr_type_name");

                entity.Property(e => e.CreateFlowAgain)
                    .HasMaxLength(50)
                    .HasColumnName("create_flow_again")
                    .HasComment("是否每次审批重新生成流程：是；否");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.FlowType)
                    .HasMaxLength(50)
                    .HasColumnName("flow_type")
                    .HasComment("流程类型：审批流程；子流程");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.Note)
                    .HasMaxLength(300)
                    .HasColumnName("note");

                entity.Property(e => e.PageViewUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("page_view_url")
                    .HasComment("查看界面地址");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("审批流类型状态：有效；失效");

                entity.Property(e => e.TableApprIdName)
                    .HasMaxLength(300)
                    .HasColumnName("table_appr_id_name")
                    .HasComment("修改表：审批id字段名称");

                entity.Property(e => e.TableName)
                    .HasMaxLength(300)
                    .HasColumnName("table_name")
                    .HasComment("修改表：表名称");

                entity.Property(e => e.TablePkName)
                    .HasMaxLength(100)
                    .HasColumnName("table_pk_name")
                    .HasComment("修改表 主键字段名称");

                entity.Property(e => e.TableStatusName)
                    .HasMaxLength(300)
                    .HasColumnName("table_status_name")
                    .HasComment("修改表：状态字段名称");

                entity.Property(e => e.TransProcName)
                    .HasMaxLength(300)
                    .HasColumnName("trans_proc_name")
                    .HasComment("调用存储过程名称");
            });

            modelBuilder.Entity<AppWorkflowLineProperty>(entity =>
            {
                entity.HasKey(x => x.ProId);

                entity.ToTable("app_workflow_line_property");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ProId).HasColumnName("pro_id");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("审批流id");

                entity.Property(e => e.LineCode)
                    .HasMaxLength(300)
                    .HasColumnName("line_code")
                    .HasComment("连接线代码");

                entity.Property(e => e.Sql)
                    .HasMaxLength(3000)
                    .HasColumnName("sql")
                    .HasComment("sql条件");
            });

            modelBuilder.Entity<AppWorkflowNodePropety>(entity =>
            {
                entity.HasKey(x => x.ProId);

                entity.ToTable("app_workflow_node_propety");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ProId).HasColumnName("pro_id");

                entity.Property(e => e.ApprCorpId)
                    .HasColumnName("appr_corp_id")
                    .HasComment("审批人公司id");

                entity.Property(e => e.ApprDeptId)
                    .HasColumnName("appr_dept_id")
                    .HasComment("审批人部门id");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("审批流ID");

                entity.Property(e => e.ApprPostId)
                    .HasColumnName("appr_post_id")
                    .HasComment("审批人岗位");

                entity.Property(e => e.ApprUserId)
                    .HasColumnName("appr_user_id")
                    .HasComment("审批人id");

                entity.Property(e => e.IsConfirmUser)
                    .HasMaxLength(10)
                    .HasColumnName("is_confirm_user")
                    .HasComment("是否取提交人");

                entity.Property(e => e.NodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("node_code")
                    .HasComment("节点代码");

                entity.Property(e => e.PageViewUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("page_view_url")
                    .HasComment("查看界面地址");

                entity.Property(e => e.Rect)
                    .HasMaxLength(300)
                    .HasColumnName("rect")
                    .HasComment("跳转模块");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type")
                    .HasComment("类型：节点、线");
            });

            modelBuilder.Entity<AppWorkflowReadTran>(entity =>
            {
                entity.HasKey(x => x.ReadTranId);

                entity.ToTable("app_workflow_read_tran");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ReadTranId).HasColumnName("read_tran_id");

                entity.Property(e => e.ApprId)
                    .HasColumnName("appr_id")
                    .HasComment("审批id");

                entity.Property(e => e.ApprTranId)
                    .HasColumnName("appr_tran_id")
                    .HasComment("审批处理id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser).HasColumnName("creation_user");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser).HasColumnName("last_modified_user");

                entity.Property(e => e.ReadCorpId)
                    .HasColumnName("read_corp_id")
                    .HasComment("阅读人公司id");

                entity.Property(e => e.ReadDate)
                    .HasColumnType("datetime")
                    .HasColumnName("read_date")
                    .HasComment("阅读时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.ReadDeptId)
                    .HasColumnName("read_dept_id")
                    .HasComment("阅读人部门id");

                entity.Property(e => e.ReadNote)
                    .HasColumnName("read_note")
                    .HasComment("阅读人意见");

                entity.Property(e => e.ReadPostId)
                    .HasColumnName("read_post_id")
                    .HasComment("阅读人岗位id");

                entity.Property(e => e.ReadUserId)
                    .HasColumnName("read_user_id")
                    .HasComment("阅读人");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("状态：未阅；已阅；阅毕");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

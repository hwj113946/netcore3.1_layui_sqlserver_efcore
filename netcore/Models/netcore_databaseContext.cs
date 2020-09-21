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
        public virtual DbSet<Appr> Apprs { get; set; }
        public virtual DbSet<ApprFlow> ApprFlows { get; set; }
        public virtual DbSet<ApprTran> ApprTrans { get; set; }
        public virtual DbSet<ApprTranTemp> ApprTranTemps { get; set; }
        public virtual DbSet<ApprType> ApprTypes { get; set; }
        public virtual DbSet<FlowLine> FlowLines { get; set; }
        public virtual DbSet<FlowLinePro> FlowLinePros { get; set; }
        public virtual DbSet<FlowNode> FlowNodes { get; set; }
        public virtual DbSet<FlowNodePro> FlowNodePros { get; set; }

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

            modelBuilder.Entity<Appr>(entity =>
            {
                entity.ToTable("appr");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ApprId)
                    .HasColumnName("appr_id")
                    .HasComment("审批id");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("审批流id");

                entity.Property(e => e.ApprNote)
                    .HasColumnName("appr_note")
                    .HasComment("审批意见");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasComment("创建时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser)
                    .HasColumnName("creation_user")
                    .HasComment("创建人");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasComment("最后修改时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser)
                    .HasColumnName("last_modified_user")
                    .HasComment("最后修改人");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasComment("审批说明");

                entity.Property(e => e.SourceId)
                    .HasColumnName("source_id")
                    .HasComment("来源id");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("状态：编辑；审批中；审批通过；审批退回");

                entity.Property(e => e.SubmissionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("submission_time")
                    .HasComment("提交时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.Submitter)
                    .HasColumnName("submitter")
                    .HasComment("提交人id");

                entity.Property(e => e.SubmitterCorp)
                    .HasColumnName("submitter_corp")
                    .HasComment("提交人公司id");

                entity.Property(e => e.SubmitterDept)
                    .HasColumnName("submitter_dept")
                    .HasComment("提交人部门id");

                entity.Property(e => e.SubmitterPhone)
                    .HasMaxLength(50)
                    .HasColumnName("submitter_phone")
                    .HasComment("提交人手机号");

                entity.Property(e => e.SubmitterPost)
                    .HasColumnName("submitter_post")
                    .HasComment("提交人岗位id");

                entity.Property(e => e.Tile)
                    .HasMaxLength(1000)
                    .HasColumnName("tile")
                    .HasComment("审批标题");
            });

            modelBuilder.Entity<ApprFlow>(entity =>
            {
                entity.ToTable("appr_flow");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("主键");

                entity.Property(e => e.ApprFlowName)
                    .HasMaxLength(300)
                    .HasColumnName("appr_flow_name")
                    .HasComment("审批流名称");

                entity.Property(e => e.ApprTypeId)
                    .HasColumnName("appr_type_id")
                    .HasComment("审批类型");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasComment("创建时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser)
                    .HasColumnName("creation_user")
                    .HasComment("创建人");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasComment("最后修改时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser)
                    .HasColumnName("last_modified_user")
                    .HasComment("最后修改人");

                entity.Property(e => e.Note)
                    .HasMaxLength(300)
                    .HasColumnName("note")
                    .HasComment("备注");
            });

            modelBuilder.Entity<ApprTran>(entity =>
            {
                entity.HasKey(x => x.TranId)
                    .HasName("PK__appr_tra__A67F8A20ADF49727");

                entity.ToTable("appr_tran");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.TranId)
                    .HasColumnName("tran_id")
                    .HasComment("事务主键");

                entity.Property(e => e.ApprId)
                    .HasColumnName("appr_id")
                    .HasComment("审批主键");

                entity.Property(e => e.ApprNote)
                    .HasColumnName("appr_note")
                    .HasComment("审批意见");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasComment("创建时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser)
                    .HasColumnName("creation_user")
                    .HasComment("创建人");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasComment("最后修改时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser)
                    .HasColumnName("last_modified_user")
                    .HasComment("最后修改人");

                entity.Property(e => e.LastSubmitNodeId)
                    .HasColumnName("last_submit_node_id")
                    .HasComment("上一个提交节点id");

                entity.Property(e => e.NextSubmitNodeId)
                    .HasColumnName("next_submit_node_id")
                    .HasComment("下一个节点id");

                entity.Property(e => e.NextSubmitNodeSubmitter)
                    .HasColumnName("next_submit_node_submitter")
                    .HasComment("下一个节点的审批人");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("状态：待审批；同意；收回；不同意（退回）");

                entity.Property(e => e.SubmissionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("submission_time")
                    .HasComment("提交时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.SubmitNodeId)
                    .HasColumnName("submit_node_id")
                    .HasComment("提交节点id");

                entity.Property(e => e.Submitter)
                    .HasColumnName("submitter")
                    .HasComment("提交人id");

                entity.Property(e => e.SubmitterCorp)
                    .HasColumnName("submitter_corp")
                    .HasComment("提交人公司id");

                entity.Property(e => e.SubmitterDept)
                    .HasColumnName("submitter_dept")
                    .HasComment("提交人部门id");

                entity.Property(e => e.SubmitterNote)
                    .HasColumnName("submitter_note")
                    .HasComment("提交意见");

                entity.Property(e => e.SubmitterPost)
                    .HasColumnName("submitter_post")
                    .HasComment("提交人岗位id");

                entity.Property(e => e.TranNumber)
                    .HasColumnName("tran_number")
                    .HasComment("处理顺序（自动增长）");
            });

            modelBuilder.Entity<ApprTranTemp>(entity =>
            {
                entity.HasKey(x => x.TranTemp);

                entity.ToTable("appr_tran_temp");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.TranTemp).HasColumnName("tran_temp");

                entity.Property(e => e.ApprFlowId).HasColumnName("appr_flow_id");

                entity.Property(e => e.ApprTypeId).HasColumnName("appr_type_id");

                entity.Property(e => e.CurrNodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("curr_node_code");

                entity.Property(e => e.CurrNodeName)
                    .HasMaxLength(300)
                    .HasColumnName("curr_node_name");

                entity.Property(e => e.CurrNodeType)
                    .HasMaxLength(50)
                    .HasColumnName("curr_node_type");

                entity.Property(e => e.CurrRect).HasColumnName("curr_rect");

                entity.Property(e => e.NextNodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("next_node_code");

                entity.Property(e => e.NextNodeType)
                    .HasMaxLength(50)
                    .HasColumnName("next_node_type");

                entity.Property(e => e.SourceId).HasColumnName("source_id");

                entity.Property(e => e.UpperNodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("upper_node_code");

                entity.Property(e => e.UpperRect).HasColumnName("upper_rect");

                entity.Property(e => e.WN).HasColumnName("w_n");
            });

            modelBuilder.Entity<ApprType>(entity =>
            {
                entity.ToTable("appr_type");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.ApprTypeId)
                    .HasColumnName("appr_type_id")
                    .HasComment("主键");

                entity.Property(e => e.ApprCancelStatus)
                    .HasMaxLength(50)
                    .HasColumnName("appr_cancel_status")
                    .HasComment("数据表审批退回的状态");

                entity.Property(e => e.ApprEndStatus)
                    .HasMaxLength(50)
                    .HasColumnName("appr_end_status")
                    .HasComment("数据表审批通过的状态");

                entity.Property(e => e.ApprStartStatus)
                    .HasMaxLength(50)
                    .HasColumnName("appr_start_status")
                    .HasComment("数据表启动审批后的状态");

                entity.Property(e => e.ApprTypeCode)
                    .HasMaxLength(100)
                    .HasColumnName("appr_type_code")
                    .HasComment("类型代码");

                entity.Property(e => e.ApprTypeName)
                    .HasMaxLength(300)
                    .HasColumnName("appr_type_name")
                    .HasComment("类型名称");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasComment("创建时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser)
                    .HasColumnName("creation_user")
                    .HasComment("创建人");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasComment("最后修改时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser)
                    .HasColumnName("last_modified_user")
                    .HasComment("最后修改人");

                entity.Property(e => e.PageViewUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("page_view_url")
                    .HasComment("审批默认查看界面：如审批节点中未填写，则取本字段");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasComment("状态：有效；无效");

                entity.Property(e => e.TableApprIdName)
                    .HasMaxLength(300)
                    .HasColumnName("table_appr_id_name")
                    .HasComment("数据表审批ID名称");

                entity.Property(e => e.TableName)
                    .HasMaxLength(300)
                    .HasColumnName("table_name")
                    .HasComment("数据表名称");

                entity.Property(e => e.TablePkName)
                    .HasMaxLength(100)
                    .HasColumnName("table_pk_name")
                    .HasComment("数据表主键名称");

                entity.Property(e => e.TableStatusName)
                    .HasMaxLength(300)
                    .HasColumnName("table_status_name")
                    .HasComment("数据表状态名称");

                entity.Property(e => e.TransProcName)
                    .HasMaxLength(300)
                    .HasColumnName("trans_proc_name")
                    .HasComment("审批通过后调用的存储过程（存储过程名称(参数)）");
            });

            modelBuilder.Entity<FlowLine>(entity =>
            {
                entity.HasKey(x => x.LineId)
                    .HasName("PK__flow_lin__F5AE5F622CF06E9F");

                entity.ToTable("flow_lines");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.LineId)
                    .HasColumnName("line_id")
                    .HasComment("主键");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("审批流id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasComment("创建时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser)
                    .HasColumnName("creation_user")
                    .HasComment("创建人");

                entity.Property(e => e.From)
                    .HasMaxLength(300)
                    .HasColumnName("from")
                    .HasComment("连接起始节点代码");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasComment("最后修改时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser)
                    .HasColumnName("last_modified_user")
                    .HasComment("最后修改人");

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
                    .HasComment("序列（用于区分连接线位置顺序）");

                entity.Property(e => e.To)
                    .HasMaxLength(300)
                    .HasColumnName("to")
                    .HasComment("连接末端节点代码");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type")
                    .HasComment("类型");
            });

            modelBuilder.Entity<FlowLinePro>(entity =>
            {
                entity.HasKey(x => x.LineProId)
                    .HasName("PK__flow_lin__D37404F08EE075A7");

                entity.ToTable("flow_line_pro");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.LineProId)
                    .HasColumnName("line_pro_id")
                    .HasComment("主键");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasComment("创建时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser)
                    .HasColumnName("creation_user")
                    .HasComment("创建人");

                entity.Property(e => e.FlowId)
                    .HasColumnName("flow_id")
                    .HasComment("审批流主键");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasComment("最后修改时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser)
                    .HasColumnName("last_modified_user")
                    .HasComment("最后修改人");

                entity.Property(e => e.LineCode)
                    .HasMaxLength(300)
                    .HasColumnName("line_code")
                    .HasComment("连接线代码");

                entity.Property(e => e.Sql)
                    .HasColumnName("sql")
                    .HasComment("连接线条件sql");
            });

            modelBuilder.Entity<FlowNode>(entity =>
            {
                entity.HasKey(x => x.NodeId)
                    .HasName("PK__flow_nod__5F19EF16AABB5AB8");

                entity.ToTable("flow_nodes");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.NodeId)
                    .HasColumnName("node_id")
                    .HasComment("主键");

                entity.Property(e => e.ApprFlowId)
                    .HasColumnName("appr_flow_id")
                    .HasComment("审批流主键");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasComment("创建时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser)
                    .HasColumnName("creation_user")
                    .HasComment("创建人");

                entity.Property(e => e.Height)
                    .HasColumnName("height")
                    .HasComment("高");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasComment("最后修改时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser)
                    .HasColumnName("last_modified_user")
                    .HasComment("最后修改人");

                entity.Property(e => e.Left).HasColumnName("left");

                entity.Property(e => e.NodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("node_code")
                    .HasComment("节点代码");

                entity.Property(e => e.NodeName)
                    .HasMaxLength(300)
                    .HasColumnName("node_name")
                    .HasComment("节点名称");

                entity.Property(e => e.Num)
                    .HasColumnName("num")
                    .HasComment("序列（用于区分节点位置顺序）");

                entity.Property(e => e.Top)
                    .HasColumnName("top")
                    .HasComment("上");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type")
                    .HasComment("节点类型");

                entity.Property(e => e.Width)
                    .HasColumnName("width")
                    .HasComment("宽");
            });

            modelBuilder.Entity<FlowNodePro>(entity =>
            {
                entity.HasKey(x => x.NodeProId)
                    .HasName("PK__flow_nod__ADFBCA059DED63CE");

                entity.ToTable("flow_node_pro");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.NodeProId)
                    .HasColumnName("node_pro_id")
                    .HasComment("主键");

                entity.Property(e => e.ApprCorpId)
                    .HasColumnName("appr_corp_id")
                    .HasComment("审批人公司：如果审批人为空时，该字段不能为空");

                entity.Property(e => e.ApprDeptId)
                    .HasColumnName("appr_dept_id")
                    .HasComment("审批人部门：部门为空：直接取整个公司");

                entity.Property(e => e.ApprPostId)
                    .HasColumnName("appr_post_id")
                    .HasComment("审批人岗位：岗位为空，直接取整个部门");

                entity.Property(e => e.ApprUserId)
                    .HasColumnName("appr_user_id")
                    .HasComment("节点审批人：如果为空，从所选择的公司、部门、岗位中全部罗列出来；如果不为空，把公司、部门、岗位一起填上");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date")
                    .HasComment("创建时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.CreationUser)
                    .HasColumnName("creation_user")
                    .HasComment("创建人");

                entity.Property(e => e.FlowId)
                    .HasColumnName("flow_id")
                    .HasComment("审批流主键");

                entity.Property(e => e.LastModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("last_modified_date")
                    .HasComment("最后修改时间")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.LastModifiedUser)
                    .HasColumnName("last_modified_user")
                    .HasComment("最后修改人");

                entity.Property(e => e.NodeCode)
                    .HasMaxLength(300)
                    .HasColumnName("node_code")
                    .HasComment("节点代码");

                entity.Property(e => e.PageViewUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("page_view_url")
                    .HasComment("节点查看界面：当审批执行到该节点时，点击标题弹出界面的地址");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

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
        public virtual DbSet<AppNews> AppNews { get; set; }
        public virtual DbSet<AppNewsType> AppNewsTypes { get; set; }
        public virtual DbSet<AppPost> AppPosts { get; set; }
        public virtual DbSet<AppRole> AppRoles { get; set; }
        public virtual DbSet<AppRoleMenu> AppRoleMenus { get; set; }
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<AppUserRole> AppUserRoles { get; set; }

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
                entity.HasNoKey();

                entity.ToTable("app_user_role");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using netcore.Models;

namespace netcore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //自定义配置文件目录
            var basePath = Directory.GetCurrentDirectory();
            CustomConfiguration = new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
        }

        public IConfiguration Configuration { get; }
        public static IWebHostEnvironment HostingEnvironment { get; internal set; }
        public IConfigurationRoot CustomConfiguration { get; }//跨域

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<netcore_databaseContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("SqlServerConnectStrings")));
            //services.AddDbContext<netcore_databaseContext>(opt => opt.UseMySql(Configuration.GetConnectionString("MySqlConnectStrings")));
            
            //使用session
            services.AddSession(o => { o.IOTimeout = TimeSpan.FromSeconds(3200); });

            //解决ViewBag的中文编码问题
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            services.AddDistributedMemoryCache();

            //将HttpClient注入进来
            services.AddHttpClient();
            //json设置
            services.AddControllersWithViews().AddNewtonsoftJson().AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                option.SerializerSettings.ContractResolver = new NullWithEmptyStringResolver();
            });

            #region jwt
            var jwtconfig = Configuration.GetSection("Jwt").Get<JwtConfig>();
            // JWT身份认证
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtconfig.Issuer,
                    ValidAudience = jwtconfig.Audience,
                    ValidateIssuer = true,
                    // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtconfig.SigningKey)),
                    // 允许的服务器时间偏移量
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });
            services.AddOptions().Configure<JwtConfig>(Configuration.GetSection("Jwt"));
            #endregion

            #region swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "netcore", Version = "v1" });
            });
            #endregion

            #region 注册跨域服务 并指定允许请求的数据源
            //注册跨域服务 并指定允许请求的数据源
            IConfiguration allowUrlSection = CustomConfiguration.GetSection("Address");
            string allowurl = "";
            if (allowUrlSection != null)
            {
                allowurl = allowUrlSection["allowUrl"];
            }
            string[] allowArray = allowurl.Split(";", StringSplitOptions.RemoveEmptyEntries);
            services.AddCors(Action => Action.AddPolicy("AllowSpecificOrigin", policy =>
            policy.WithOrigins(allowArray)
            .AllowAnyHeader()
            .AllowAnyMethod() //如果不加此方法 默认只允许Get方式跨域请求
            // .AllowCredentials() // 指定处理cookie
            ));
            #endregion

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseRouting();
            //认证
            app.UseAuthentication();
            //身份验证
            app.UseAuthorization();
            //使用session
            app.UseSession();
            //启用跨域
            app.UseCors("AllowSpecificOrigin");
            //使用fastreport
            app.UseFastReport();
            //使用Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "netcore_ef v1");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhaoxi.AgileFramework.Common.IOCOptions;
using Zhaoxi.AgileFramework.WebCore.FilterExtend;
using Zhaoxi.AgileFramework.WebCore.MiddlewareExtend;
using Zhaoxi.MSACommerce.Core;
using Zhaoxi.MSACommerce.Interface;
using Zhaoxi.MSACommerce.Model;
using Zhaoxi.MSACommerce.Service;

namespace Zhaoxi.MSACommerce.UserMicroservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Filter
            services.AddControllers(o =>
            {
                o.Filters.Add(typeof(CustomExceptionFilterAttribute));
                o.Filters.Add(typeof(LogActionFilterAttribute));
            });
            #endregion
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Zhaoxi.MSACommerce.UserMicroservice", Version = "v1" });
            });
            #region ����ע��
            services.AddTransient<OrangeContext, OrangeContext>();
            services.AddTransient<CacheClientDB, CacheClientDB>();
            services.AddTransient<IUserService, UserService>();
            #endregion

            #region �����ļ�ע��
            services.Configure<MySqlConnOptions>(this.Configuration.GetSection("MysqlConn"));
            services.Configure<RedisConnOptions>(this.Configuration.GetSection("RedisConn"));
            #endregion

            #region jwtУ��  HS
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            Configuration.Bind("JWTTokenOptions", tokenOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
                    ValidateIssuer = true,//�Ƿ���֤Issuer
                    ValidateAudience = true,//�Ƿ���֤Audience
                    ValidateLifetime = false,//�Ƿ���֤ʧЧʱ��
                    ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                    ValidAudience = tokenOptions.Audience,//
                    ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
                };
            });
            #endregion
            #region ����
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("default", policy =>
            //    {
            //        policy.AllowAnyOrigin()
            //            .AllowAnyHeader()
            //            .AllowAnyMethod();
            //    });
            //});
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zhaoxi.MSACommerce.UserMicroservice v1"));
            //}
            app.UsePreOptionsRequest();

            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            //app.UseCors("default");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

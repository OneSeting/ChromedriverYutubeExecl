using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using RestSharp;

namespace yutubeTapra
{

    public static class CreateDB
    {
        private static string Conn = "";


        private static string Table = "`Unibone_dev`";
        /// <summary>
        /// 连接数据库 插入数据
        /// </summary>
        /// <returns></returns>
        public static string ConnsqlChnnel(string url, string keyId, string ChannelId, DataRow dataRow,string link,string path,int i)
        {
            string messgae = string.Empty;
            try
            {              
                if (string.IsNullOrEmpty(keyId))
                    keyId = "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8";

                var restClient = new RestClient(url);
                var request = new RestRequest($"/youtubei/v1/browse?key={keyId}", Method.POST);
                request.AddJsonBody("{ \"context\": { \"client\": { \"clientName\": \"WEB\",\"clientVersion\": \"2.20210301.08.00\"} },\"browseId\": \"" + ChannelId + "\"}");
                var response = restClient.Execute(request);
                var ytRoot = JsonConvert.DeserializeObject<YtRoot>(response.Content);
                var subscribersString = ytRoot?.header?.c4TabbedHeaderRenderer?.subscriberCountText?.simpleText;
                decimal subscribers = 0;
                if (!string.IsNullOrWhiteSpace(subscribersString))
                {
                    subscribersString = subscribersString.Split(' ')[0];
                    if (subscribersString.ToLower().EndsWith("k"))
                    {
                        subscribersString = subscribersString.ToLower().TrimEnd('k');
                        subscribers = Convert.ToDecimal(subscribersString) * 1000;
                    }
                    else if (subscribersString.ToLower().EndsWith("m"))
                    {
                        subscribersString = subscribersString.ToLower().TrimEnd('m');
                        subscribers = Convert.ToDecimal(subscribersString) * 1000000;
                    }
                }

                if (null != ytRoot.contents)
                {
                    DateTime utcNow = Convert.ToDateTime(dataRow["DATE"].ToString());
                    //构造yutube
                    YoutubeUser youtubeUser = new YoutubeUser()
                    {
                        Subscribers = (int)subscribers, //最大值超过10亿够用
                        Description = ytRoot.microformat?.microformatDataRenderer?.description,
                        Link = $"{url}{ytRoot.header?.c4TabbedHeaderRenderer?.navigationEndpoint?.commandMetadata?.webCommandMetadata?.url}",
                        Channel = ytRoot.header?.c4TabbedHeaderRenderer?.title,

                        CreatedOnUtc = utcNow,
                        CrawledOnUtc = utcNow,
                        Creator = dataRow["CREATOR"].ToString(),
                        Operator = dataRow["CREATOR"].ToString(),
                        KeyId = keyId,
                        ChannelId = ChannelId,
                        Email = dataRow["EMAIL"].ToString() == "空" ? "" : dataRow["EMAIL"].ToString(),
                        KolId = 0
                    };

                    //构造kol
                    Kol kol = new Kol()
                    {
                        FullName = "",
                        Email = dataRow["EMAIL"].ToString(),
                        Tags = "",
                        Location = "",
                        IsIgUser = false,
                        IsYtUser = true,
                        Creator = youtubeUser.Operator,
                        Operator = youtubeUser.Operator,
                        UpdateBy = youtubeUser.Operator,
                        CreatedOnUtc = utcNow,
                        UpdatedOnUtc = utcNow,

                        FirstCollabOnUtc = null,
                        //CooperationStatusId =,

                        //IsManageAccount =,
                        //IsPaid =,
                        Note = dataRow["EMAIL1"].ToString(),
                        Brands = null,
                        Rate = null
                    };

                    //构造网红事件表
                    KolEvent kolEvent = new KolEvent()
                    {
                        Title = $"Dig {kol.FullName}",
                        KolEventTypeId = 10,
                        PlatformTypeId = 20,
                        Note = youtubeUser.Link,
                        KolId = 0,
                        CreatedOnUtc = utcNow,
                        UpdatedOnUtc = utcNow,
                        EventOnUtc = utcNow
                    };
                    messgae = CreateMysqls(ChannelId, youtubeUser, kol, kolEvent, path, i);
                }
                else
                {
                    return messgae = "该channle不存在";
                }
            }         
            catch (Exception ex)
            {
                return ex.ToString();             
            }
            return messgae;
        }

        public static string CreateMysqls(string ChannelId, YoutubeUser youtubeUser, Kol kol, KolEvent kolEvent,string path,int i)
        {
            //String connetStr = "server=127.0.0.1;user=root;password=root;database=minecraftdb;";
            MySqlConnection conn = new MySqlConnection(Conn);
            conn.Open();//必须打开通道之后才能开始事务
            MySqlTransaction transaction = conn.BeginTransaction();//事务必须在try外面赋值不然catch里的transaction会报错:未赋值
            Console.WriteLine("已经建立连接");
            try
            {
                //先查询判断当前的ChannelId 是否存在
                MySqlCommand cmd0 = new MySqlCommand($"Select Id from YoutubeUser where ChannelId='{ChannelId}'", conn);
                MySqlDataReader mysdr = cmd0.ExecuteReader();
                while (mysdr.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                {
                    if (!string.IsNullOrEmpty(mysdr[0].ToString()))
                    {
                        mysdr.Close();
                        mysdr.Dispose();
                        return "以重复";
                    }
                }
                mysdr.Close();
                mysdr.Dispose();

                string sql1 = @$"INSERT INTO `Kol`(
                                                    `FullName`,
                                                    `Email`, 
                                                    `Location`,
                                                    `Rate`,
                                                    `IsPaid`,
                                                    `CooperationStatusId`,
                                                    `IsIgUser`,
                                                    `IsYtUser`,
                                                    `IsManageAccount`,
                                                    `Tags`,
                                                    `Brands`,
                                                    `Note`,
                                                    `Creator`,
                                                    `Operator`,
                                                    `FirstCollabOnUtc`,
                                                    `CreatedOnUtc`,
                                                    `UpdatedOnUtc`,
                                                    `UpdateBy`
                                                    ) 
                     VALUES (@FullName, @Email, @Location,@Rate,@IsPaid,@CooperationStatusId,@IsIgUser, @IsYtUser, @IsManageAccount,
                             @Tags, @Brands, @Note, @Creator, @Operator, @FirstCollabOnUtc, @CreatedOnUtc, @UpdatedOnUtc, @UpdateBy);";
                MySqlCommand cmd1 = new MySqlCommand(sql1, conn);
                cmd1.Parameters.Add(new MySqlParameter("@FullName", kol.FullName));
                cmd1.Parameters.Add(new MySqlParameter("@Email", kol.Email));
                cmd1.Parameters.Add(new MySqlParameter("@Location", kol.Location));
                cmd1.Parameters.Add(new MySqlParameter("@Rate", kol.Rate));
                cmd1.Parameters.Add(new MySqlParameter("@IsPaid", kol.IsPaid));
                cmd1.Parameters.Add(new MySqlParameter("@CooperationStatusId", kol.CooperationStatusId));
                cmd1.Parameters.Add(new MySqlParameter("@IsIgUser", kol.IsIgUser));
                cmd1.Parameters.Add(new MySqlParameter("@IsYtUser", kol.IsYtUser));
                cmd1.Parameters.Add(new MySqlParameter("@IsManageAccount", kol.IsManageAccount));
                cmd1.Parameters.Add(new MySqlParameter("@Tags", kol.Tags));
                cmd1.Parameters.Add(new MySqlParameter("@Brands", kol.Brands));
                cmd1.Parameters.Add(new MySqlParameter("@Note", kol.Note));
                cmd1.Parameters.Add(new MySqlParameter("@Creator", kol.Creator));
                cmd1.Parameters.Add(new MySqlParameter("@Operator", kol.Operator));
                cmd1.Parameters.Add(new MySqlParameter("@FirstCollabOnUtc", kol.FirstCollabOnUtc));
                cmd1.Parameters.Add(new MySqlParameter("@CreatedOnUtc", kol.CreatedOnUtc));
                cmd1.Parameters.Add(new MySqlParameter("@UpdatedOnUtc", kol.UpdatedOnUtc));
                cmd1.Parameters.Add(new MySqlParameter("@UpdateBy", kol.UpdateBy));
                cmd1.ExecuteNonQuery();


                MySqlCommand cmd = new MySqlCommand("SELECT LAST_INSERT_ID()", conn);
                int kolId = int.Parse(cmd.ExecuteScalar().ToString()); //执行ExecuteReader()返回一个MySqlDataReader对象

                kolEvent.KolId = kolId;
                youtubeUser.KolId = kolId;

                string sql2 = @$"
                    INSERT INTO `KolEvent`( `Title`, 
                                            `KolEventTypeId`,
                                            `PlatformTypeId`,
                                            `Note`, `KolId`,
                                            `EventOnUtc`,
                                            `CreatedOnUtc`,
                                            `UpdatedOnUtc`)
                    VALUES (@Title,@KolEventTypeId ,@PlatformTypeId , @Note,@KolId ,@EventOnUtc, @CreatedOnUtc, @UpdatedOnUtc);";
                MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                cmd2.Parameters.Add(new MySqlParameter("@Title", kolEvent.Title));
                cmd2.Parameters.Add(new MySqlParameter("@KolEventTypeId", kolEvent.KolEventTypeId));
                cmd2.Parameters.Add(new MySqlParameter("@PlatformTypeId", kolEvent.PlatformTypeId));
                cmd2.Parameters.Add(new MySqlParameter("@Note", kolEvent.Note));
                cmd2.Parameters.Add(new MySqlParameter("@KolId", kolEvent.KolId));
                cmd2.Parameters.Add(new MySqlParameter("@EventOnUtc", kolEvent.EventOnUtc));
                cmd2.Parameters.Add(new MySqlParameter("@CreatedOnUtc", kolEvent.CreatedOnUtc));
                cmd2.Parameters.Add(new MySqlParameter("@UpdatedOnUtc", kolEvent.UpdatedOnUtc));
                cmd2.ExecuteNonQuery();

                #region 
                //string sql3 = @$"
                //    INSERT INTO {Table}.`YoutubeUser`(`Channel`, `Link`, `Subscribers`, `Videos`, `Description`,
                //    `CreatedOnUtc`, `UpdatedOnUtc`, `CrawledOnUtc`, `Creator`,
                //    `Operator`, `KolId`, `ChannelId`, `Email`, `KeyId`) 
                //     VALUES('{youtubeUser.Channel}', '{youtubeUser.Link}', {youtubeUser.Subscribers}, {youtubeUser.Videos}, '{youtubeUser.Description}',
                //    '{youtubeUser.CreatedOnUtc}', '{youtubeUser.UpdatedOnUtc}', '{youtubeUser.CrawledOnUtc}', '{youtubeUser.Creator}',
                //    '{youtubeUser.Operator}', {youtubeUser.KolId}, '{youtubeUser.ChannelId}', '{youtubeUser.Email}', '{youtubeUser.KeyId}'); ";
                #endregion
                string sql3 = @$"
                    INSERT INTO `YoutubeUser`(  `Channel`,
                                                `Link`,
                                                `Subscribers`,
                                                `Videos`,
                                                `Description`,
                                                `CreatedOnUtc`,
                                                `UpdatedOnUtc`,
                                                `CrawledOnUtc`,
                                                `Creator`,
                                                `Operator`,
                                                `KolId`,
                                                `ChannelId`,
                                                `Email`,
                                                `KeyId`) 
                    VALUES(@Channel, @Link,@Subscribers ,@Videos ,@Description,@CreatedOnUtc, @UpdatedOnUtc, @CrawledOnUtc,@Creator,
                           @Operator,@KolId , @ChannelId, @Email, @KeyId); ";
                MySqlCommand cmd3 = new MySqlCommand(sql3, conn);
                cmd3.CommandText = sql3;
                // 下面三行是处理可能包含特殊字符的代码
                cmd3.Parameters.Add(new MySqlParameter("@Channel", youtubeUser.Channel));
                cmd3.Parameters.Add(new MySqlParameter("@Link", youtubeUser.Link));
                cmd3.Parameters.Add(new MySqlParameter("@Subscribers", youtubeUser.Subscribers));
                cmd3.Parameters.Add(new MySqlParameter("@Videos", youtubeUser.Videos));
                cmd3.Parameters.Add(new MySqlParameter("@Description", youtubeUser.Description));
                cmd3.Parameters.Add(new MySqlParameter("@CreatedOnUtc", youtubeUser.CreatedOnUtc));
                cmd3.Parameters.Add(new MySqlParameter("@UpdatedOnUtc", youtubeUser.UpdatedOnUtc));
                cmd3.Parameters.Add(new MySqlParameter("@CrawledOnUtc", youtubeUser.CrawledOnUtc));
                cmd3.Parameters.Add(new MySqlParameter("@Creator", youtubeUser.Creator));
                cmd3.Parameters.Add(new MySqlParameter("@Operator", youtubeUser.Operator));
                cmd3.Parameters.Add(new MySqlParameter("@KolId", youtubeUser.KolId));
                cmd3.Parameters.Add(new MySqlParameter("@ChannelId", youtubeUser.ChannelId));
                cmd3.Parameters.Add(new MySqlParameter("@Email", youtubeUser.Email));
                cmd3.Parameters.Add(new MySqlParameter("@KeyId", youtubeUser.KeyId));
                cmd3.ExecuteNonQuery();

                transaction.Commit();
                Console.WriteLine("插入数据库成功");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                transaction.Rollback();//事务ExecuteNonQuery()执行失败报错，username被设置unique
                conn.Close();
                throw new AggregateException(ex.Message);
            }
            finally
            {
                transaction.Dispose();
                conn.Dispose();
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }          
            }
            return "以插入成功";
        }
    }
}

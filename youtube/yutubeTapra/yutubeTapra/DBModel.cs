using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace yutubeTapra
{
    public class Kol
    {
        /// <summary>
        /// 网红真实全名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 所在地
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 网红评级
        /// </summary>
        public string Rate { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 是否付费合作关系
        /// </summary>
        public bool IsPaid { get; set; } = false;

        /// <summary>
        /// 合作状态
        /// </summary>
        public int CooperationStatusId { get; set; } = 20;

        /// <summary>
        /// 是否IG用户
        /// </summary>
        public bool IsIgUser { get; set; }

        /// <summary>
        /// 是否公司运营账号
        /// </summary>
        public bool IsManageAccount { get; set; } = false;

        /// <summary>
        /// 是否YT用户
        /// </summary>
        public bool IsYtUser { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brands { get; set; }

        /// <summary>
        /// 标注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 初次合作邮件日期
        /// </summary>
        public DateTime? FirstCollabOnUtc { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }
    }

    public class KolEvent
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public int KolEventTypeId { get; set; }

        /// <summary>
        /// 社交平台类型
        /// </summary>
        public int PlatformTypeId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 根据事件类型，存储一些相关的外键，
        /// 比如事件类型是邮件相关，那么存储EmailQueue的某条邮件Id
        /// 具体视业务而定
        /// </summary>
        //public string MetaId { get; set; }

        /// <summary>
        /// 网红Id
        /// </summary>
        public int? KolId { get; set; }

        /// <summary>
        /// 事件时间
        /// </summary>
        public DateTime EventOnUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// YoutubeUser 实体类
    /// </summary>
    public class YoutubeUser
    {

        /// <summary>
        /// KolId
        /// </summary>
        public int? KolId { get; set; }

        /// <summary>
        ///频道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 订阅者
        /// </summary>
        public int? Subscribers { get; set; }

        /// <summary>
        /// 视频
        /// </summary>
        public int? Videos { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedOnUtc { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedOnUtc { get; set; }

        /// <summary>
        /// 抓取时间
        /// </summary>
        public DateTime? CrawledOnUtc { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// youtube用户keyId
        /// </summary>
        public string KeyId { get; set; }
        /// <summary>
        /// 频道Id
        /// </summary>
        public string ChannelId { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; }
    }


    public class YtRoot
    {

        public Contents contents { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Header header { get; set; }

        public Microformat microformat { get; set; }

    }
    public class Contents
    {
        /// <summary>
        /// 
        /// </summary>
        public TwoColumnBrowseResultsRenderer twoColumnBrowseResultsRenderer { get; set; }
    }
    public class TwoColumnBrowseResultsRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        public List<TabsItem> tabs { get; set; }
    }
    public class TabsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public TabRenderer tabRenderer { get; set; }
    }
    public class TabRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        public Endpoint endpoint { get; set; }
        /// <summary>
        /// 首页
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string selected { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string trackingParams { get; set; }
    }
    public class Endpoint
    {
        /// <summary>
        /// 
        /// </summary>
        public string clickTrackingParams { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CommandMetadata commandMetadata { get; set; }
        /// <summary>
        /// 
        /// </summary>
    }
    public class CommandMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        public WebCommandMetadata webCommandMetadata { get; set; }
    }
    public class WebCommandMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string webPageType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int rootVe { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string apiUrl { get; set; }
    }

    public class Microformat
    {
        /// <summary>
        /// 
        /// </summary>
        public MicroformatDataRenderer microformatDataRenderer { get; set; }
    }
    public class MicroformatDataRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }

    }

    public class Header
    {
        /// <summary>
        /// 
        /// </summary>
        public C4TabbedHeaderRenderer c4TabbedHeaderRenderer { get; set; }
    }

    public class C4TabbedHeaderRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        public string channelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SubscriberCountText subscriberCountText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public NavigationEndpoint navigationEndpoint { get; set; }
    }
    public class NavigationEndpoint
    {
        /// <summary>
        /// 
        /// </summary>
        public string clickTrackingParams { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CommandMetadata commandMetadata { get; set; }

    }
    public class SubscriberCountText
    {
        /// <summary>
        /// 250位订阅者
        /// </summary>
        public string simpleText { get; set; }
    }





}

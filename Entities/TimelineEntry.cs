using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Entities
{
    public enum TimelineDataCategory
    {
        /// <summary>
        /// コメント（旧形式）
        /// </summary>
        Comment = 1,
        /// <summary>
        /// テキスト（はてなのユーザーに公開，旧形式）
        /// </summary>
        TextForHatenaUsers = 2,
        /// <summary>
        /// [廃止]「ともだちになりました」通知
        /// </summary>
        [Obsolete] BecomeFriend = 3,
        /// <summary>
        /// [廃止] はてなココのイマココ
        /// </summary>
        [Obsolete] Imakoko = 10,
        /// <summary>
        /// [廃止] はてなココのスポットのレビュー
        /// </summary>
        [Obsolete] SpotReview = 11,
        /// <summary>
        /// テキスト（全体に公開，旧形式）
        /// </summary>
        PublicText = 14,
        /// <summary>
        /// テキスト（はてなハイク2ユーザーに公開，旧形式）
        /// </summary>
        Hike2Text = 15,
        /// <summary>
        /// テキスト（ともだちのともだちまで公開，旧形式）
        /// </summary>
        TextForAcquaintances = 16,
        /// <summary>
        /// テキスト（ともだちに公開，旧形式）
        /// </summary>
        TextForFriends = 17,
        /// <summary>
        /// テキスト（指定した範囲のユーザーに公開）
        /// </summary>
        TextForAnyRange = 19,
        /// <summary>
        /// テキスト（ユーザーごとの公開範囲設定に基づき公開）
        /// </summary>
        Text = 20,
        /// <summary>
        /// はてなOneのコメント
        /// </summary>
        HatenaOneComment = 30,
        /// <summary>
        /// ハッピィの編集
        /// </summary>
        EdittedHappy = 31,
        /// <summary>
        /// アプリケーション利用開始の通知
        /// </summary>
        ApplicationStartNotice = 43,
        /// <summary>
        /// はてなプラス利用開始の通知
        /// </summary>
        HatenaPlusStartNotice = 45,
        /// <summary>
        /// [廃止] うごメモの作品
        /// </summary>
        [Obsolete] UgoMemo = 50,
        /// <summary>
        /// はてなハイクの投稿
        /// </summary>
        Hike = 62,

        /// <summary>
        /// はてなブックマークのブックマーク
        /// </summary>
        Bookmark = 87,

        /// <summary>
        /// はてなココのエントリー（？）
        /// </summary>
        KokoEntry = 88,

        /// <summary>
        /// はてなブログの投稿
        /// </summary>
        Blog = 97,
        /// <summary>
        /// はてなダイアリーの投稿
        /// </summary>
        Diary = 112,
    }

    public class TimelineEntry
    {
        /// <summary>
        /// アプリケーションアイコンのURL
        /// </summary>
        [JsonProperty("app_icon_url")]
        public string AppIconUrl { get; set; }

        /// <summary>
        /// アプリケーションロゴのURL
        /// </summary>
        [JsonProperty("app_logo_url")]
        public string AppLogoUrl { get; set; }

        /// <summary>
        /// アプリケーション名
        /// </summary>
        [JsonProperty("app_name")]
        public string AppName { get; set; }

        /// <summary>
        /// アプリケーションのURL
        /// </summary>
        [JsonProperty("app_url")]
        public string AppUrl { get; set; }

        /// <summary>
        /// エントリーの著者
        /// </summary>
        [JsonProperty("author")]
        public TargetObject Author { get; set; }

        /// <summary>
        /// 本文テキスト（はてなブックマーク用はてな記法）
        /// </summary>
        [JsonProperty("body_bookmark_text")]
        public string BodyBookmarkText { get; set; }

        /// <summary>
        /// 本文テキスト（はてなダイアリー用はてな記法）
        /// </summary>
        [JsonProperty("body_diary_text")]
        public string BodyDiaryText { get; set; }

        /// <summary>
        /// 本文テキスト（はてなハイク用はてな記法）
        /// </summary>
        [JsonProperty("body_haiku_text")]
        public string BodyHaikuText { get; set; }

        /// <summary>
        /// 本文テキスト
        /// </summary>
        [JsonProperty("body_text")]
        public string BodyText { get; set; }

        /// <summary>
        /// ブックマークエントリーのタイトル
        /// </summary>
        [JsonProperty("bookmark_entry_title")]
        public string BookmarkEntryTitle { get; set; }

        /// <summary>
        /// ブックマークエントリーのURL
        /// </summary>
        [JsonProperty("bookmark_entry_url")]
        public string BookmarkEntryUrl { get; set; }

        /// <summary>
        /// 投稿日時
        /// </summary>
        [JsonProperty("created_on")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// エントリーの種別
        /// </summary>
        [JsonProperty("data_category")]
        public TimelineDataCategory Category { get; set; }

        /// <summary>
        /// エントリーID
        /// </summary>
        [JsonProperty("eid")]
        public long Id { get; set; }

        /// <summary>
        /// 品物ID
        /// </summary>
        [JsonProperty("mono_key")]
        public string MonoKey { get; set; }

        /// <summary>
        /// 写真のURL
        /// </summary>
        [JsonProperty("photo_url")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 写真の縮小版のURL
        /// </summary>
        [JsonProperty("photo_small_url")]
        public string PhotoSmallUrl { get; set; }

        /// <summary>
        /// 質問文の要約
        /// </summary>
        [JsonProperty("question_summary_text")]
        public string QuestionSummaryText { get; set; }

        /// <summary>
        /// 質問ページのURL
        /// </summary>
        [JsonProperty("question_url")]
        public string QuestionUrl { get; set; }

        /// <summary>
        /// 親エントリーの著者
        /// </summary>
        [JsonProperty("reply_to_author")]
        public TargetObject ReplyToAuthor { get; set; }

        /// <summary>
        /// 親エントリーのエントリーID
        /// </summary>
        [JsonProperty("reply_to_eid")]
        public long ReplyToId { get; set; }

        /// <summary>
        /// 投稿元アプリケーション
        /// </summary>
        [JsonProperty("source_target")]
        public TargetObject SourceTarget { get; set; }

        /// <summary>
        /// スポットの評価点
        /// </summary>
        [JsonProperty("spot_review_score")]
        public string SpotReviewScore { get; set; }

        /// <summary>
        /// エントリーのパーマリンクURL
        /// </summary>
        [JsonProperty("star_url")]
        public string StarUrl { get; set; }

        /// <summary>
        /// 要約文
        /// </summary>
        [JsonProperty("summary_text")]
        public string SummaryText { get; set; }

        /// <summary>
        /// エントリーの言及対象
        /// </summary>
        [JsonProperty("target")]
        public TargetObject Target { get; set; }
    }

    public class TimelineResponse
    {
        [JsonProperty("items")]
        public TimelineEntry[] Items { get; set; }

        [JsonProperty("newer_url")]
        public string NewerUrl { get; set; }

        [JsonProperty("older_url")]
        public string OlderUrl { get; set; }
    }
}

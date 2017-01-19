using IRMS_MARKERLib;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TS50SDK;

namespace TornadoSDK.Controllers {
    public class HomeController : Controller {

        //建立物件
        Ts5Core ts5Core = new Ts5Core(); //取得各項服務元件
        SimpleCriteria criteria = new SimpleCriteria(); //建立搜尋條件設定
        SearchService Searchservice; //進行搜尋服務物件
        //FieldFilter filter = new FieldFilter(); //建立過濾欄位物件
        Order order = new Order(); //建立排序相關物件
        readonly string INI = @"C:\Program Files\Tornado\Tornado Search Platform SDK\Config\TSPSDK.ini"; //讀取INI設定檔

        //初始化設定啟動搜尋功能
        public HomeController() {
            ts5Core.Initialize(INI); //將ini檔初始化
            Searchservice = ts5Core.GetSearchService(); //取得搜尋服務
            Searchservice.SetResultMark("<font color = red>", "</font>"); //標記關鍵字

            //開啟同音字搜尋功能
            criteria.Add(SearchCriteria.CRITERIA_HOMOPHONE, true);

            //設定關鍵字(自動簡繁轉換)
            criteria.Add(SearchCriteria.CRITERIA_CHINESE_CONVERT, true);

            //設定來源
            //criteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, "Database,LineMessagePush");
            criteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, "Database");

            //排序條件使用精準度且按照降冪的方式排列
            order.SetOrderType("(1)", OrderType.ORDER_TYPE_DES);
            criteria.Add(SearchCriteria.CRITERIA_ORDER, order); //將排序條件加入至搜尋條件
        }

        // GET: 一般搜尋
        public ActionResult Index() {
            return View();
        }

        [HttpGet]
        public ActionResult getSearchResult(string searchValue, int searchModeCode) {
            ResultList resultList = null; //建立搜尋結果列表
            ResultItem resultitem;//建立搜尋結果項目
            List<object> listResult = new List<object>();
            string querySeconds = "0";

            //自訂關鍵字標記物件
            Marker marker = new Marker();
            marker.SetOption(0, searchValue);
            marker.SetOption(1, "<font color = red>");
            marker.SetOption(2, "</font>");

            if (!string.IsNullOrEmpty(searchValue)) {

                //設定關鍵字
                criteria.Add(SearchCriteria.CRITERIA_KEYWORD, searchValue);

                //搜尋條件
                criteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, (SearchMode)searchModeCode);

                try {

                    //設定SearchService (SimpleCriteria, 0, 10) //(條件,起始位置,回傳最大的資料筆數)
                    resultList = Searchservice.SearchBySimpleCriteria(criteria, 0); //進階搜尋
                    if (resultList == null) {
                        throw new Exception(Searchservice.GetErrorMessage());
                    }

                    int Vsize = resultList.GetValidSize(); //取得回傳的筆數
                    int Tsize = resultList.GetTotalSize(); //取得總筆數
                    querySeconds = resultList.GetQueryTime(); //取回花費秒數

                    //目前取回的值
                    for (int i = 0; i < resultList.GetSize(); i++) {
                        resultitem = resultList.GetItem(i);

                        listResult.Add(new {
                            id = resultitem.GetDocumentKey(),
                            keyword = resultitem.GetKeywords(),     //取得關鍵字
                            summary = resultitem.GetContent(),      //取得內文摘要
                                                                    //summary = resultitem.GetCustomContent("我是一個龍捲風"),      //利用自訂內容與關鍵字進行比對

                            //取得加入索引的其他欄位資料
                            //利用 marker api 加入欄位標記符號
                            title = marker.MarkText(resultitem.GetFieldByNameAsString("TITLE")),
                            description = marker.MarkText(resultitem.GetFieldByNameAsString("DESCRIPTION")),
                            //year = resultitem.GetFieldByNameAsString("YEAR"),
                            //create_date = Convert.ToDateTime(resultitem.GetFieldByNameAsString("CREATE_DATE")).ToString("yyyy-MM-dd HH:mm:ss")
                            //title = "",
                            //description = "",
                            //year = "",
                            //create_date = ""
                        });
                    }
                } catch (Exception ex) {
                    throw;
                } finally {
                    if (resultList != null) {
                        resultList.Close();
                    }
                }
            }

            var searchResultModel = new {
                query_seconds = querySeconds,
                search_results = listResult
            };

            return Json(searchResultModel, JsonRequestBehavior.AllowGet);
        }

        // GET: 同音字
        //public ActionResult Index() {

        //    //建立TermManager物件，並且指定設定檔ini路徑。
        //    Ts5Core ts5Core = new Ts5Core(); //建立各項服務元件
        //    TermManager TManager = new TermManager(); //建立各類詞彙管
        //    StringList TMHomophone; //存放多筆字串的資料結構
        //    string ini = @"C:\Program Files\Tornado\Tornado Search Platform SDK\Config\TSPSDK.ini";
        //    ts5Core.Initialize(ini);
        //    TManager = ts5Core.GetTermManager(); //取得TermManger服務
        //                                         //設定同音詞
        //    TMHomophone = TManager.ListHomophone("國定價日");
        //    int size = TMHomophone.GetSize();
        //    List<string> listResult = new List<string>();

        //    //顯示同音詞
        //    for (short i = 0; i < size; i++) {
        //        listResult.Add(TMHomophone.GetElement(i));
        //    }

        //    return View();
        //}

        // GET: 同義詞
        //public ActionResult Index() {

        //    //建立TermManager物件，並且指定設定檔ini路徑。
        //    Ts5Core ts5Core = new Ts5Core(); //建立各項服務元件
        //    TermManager TManager = new TermManager(); //建立各類詞彙管理
        //    StringList TMSynonym; //存放多筆字串的資料結構
        //    string ini = @"C:\Program Files\Tornado\Tornado Search Platform SDK\Config\TSPSDK.ini";
        //    ts5Core.Initialize(ini);
        //    TManager = ts5Core.GetTermManager(); //取得TermManger服務

        //    //寫入方法為TermManager.SetSynonym(主詞,同義詞)
        //    //TManager.SetSynonym("龍捲風", " Twister");

        //    List<string> listResult = new List<string>();
        //    TMSynonym = TManager.ListSynonym("國定價日");
        //    for (short i = 0; i < TMSynonym.GetSize(); i++) {
        //        listResult.Add(TMSynonym.GetElement(i));
        //    }

        //    return View();
        //}

        //// GET: 自動完成
        //public ActionResult Index() {

        //    //建立TermManager物件，並且指定設定檔ini路徑。
        //    Ts5Core ts5Core = new Ts5Core(); //建立各項服務元件
        //    TermManager TManager = new TermManager(); //建立各類詞彙管理
        //    StringList TMWildcard; //存放多筆字串的資料結構
        //    string ini = @"C:\Program Files\Tornado\Tornado Search Platform SDK\Config\TSPSDK.ini";
        //    ts5Core.Initialize(ini);
        //    TManager = ts5Core.GetTermManager(); //取得TermManger服務

        //    //寫入方法為TermManager.SetSynonym(主詞,同義詞)
        //    //TManager.SetSynonym("龍捲風", " Twister");

        //    //設定自動完成
        //    List<string> listResult = new List<string>();
        //    TMWildcard = TManager.ListByWildcard("地震");
        //    for (short i = 0; i < TMWildcard.GetSize(); i++) {
        //        listResult.Add(TMWildcard.GetElement(i));
        //    }

        //    return View();
        //}

        // GET: 熱門關鍵字(新增刪除)
        //public ActionResult Index() {

        //    //建立TermManager物件、 HotItemList物件，並且指定設定檔ini路徑。
        //    Ts5Core ts5Core = new Ts5Core(); //建立各項服務元件
        //    TermManager TManager = new TermManager(); //建立各類詞彙管理
        //    HotItemList HotKeyWordList = new HotItemList();//建立可存放Hotkeyword結構
        //    string ini = @"C:\Program Files\Tornado\Tornado Search Platform SDK\Config\TSPSDK.ini";

        //    //初始化並取得TermManager服務
        //    ts5Core.Initialize(ini);
        //    TManager = ts5Core.GetTermManager();
        //    //新增熱門關鍵字以及查詢次數: 例如:("龍捲風", 150)
        //    //TManager.SetHotKeyword("龍捲風", 150);//設定熱門關鍵字。 預設被點擊次數。
        //    TManager.IncreaseHotKeywordByOne("龍捲風"); //遞增熱門關鍵字的點擊數

        //    //刪除關鍵字會將熱門關鍵字刪除,同時會一併刪除查詢次數
        //    //TManager.RemoveHotKeyword("龍捲風");

        //    //給一個日期參數作為日期的範圍,並且List出在日期範圍內的熱門關鍵字
        //    HotKeyWordList = TManager.ListHotKeyword(DateTime.Parse("2012-05-27"), DateTime.Now);

        //    //利用GetSize取得關鍵字的總數I
        //    List<string> listResult = new List<string>();
        //    for (short i = 0; i < HotKeyWordList.GetSize(); i++) {
        //        //利用GetElment取得i的回傳值
        //        HotKeyword hot = (HotKeyword)HotKeyWordList.GetElement(i);
        //        listResult.Add("<br>" + hot.GetTerm() + "<br>" + hot.GetHits());
        //    }

        //    return View();
        //}

        //    // GET: Home
        //    public ActionResult Index()
        //    {
        //        //建立物件
        //        Ts5Core ts5Core = new Ts5Core(); //取得各項服務元件
        //        SimpleCriteria criteria = new SimpleCriteria(); //建立搜尋條件設定
        //        SearchService Searchservice; //進行搜尋服務物件
        //        ResultList resultList; //建立搜尋結果列表
        //        ResultItem resultitem;//建立搜尋結果項目
        //        FieldFilter filter = new FieldFilter(); //建立過濾欄位物件
        //        Order order = new Order(); //建立排序相關物件
        //                                   //讀取INI設定檔
        //        string ini = @"C:\Program Files\Tornado\Tornado Search Platform SDK\Config\TSPSDK.ini";

        //        //初始化設定啟動搜尋功能
        //        ts5Core.Initialize(ini); //將ini 檔初始化
        //        Searchservice = ts5Core.GetSearchService(); //取得搜尋服
        //        Searchservice.SetResultMark("<font color = red>", "</font>"); //標記關鍵字

        //        //設定關鍵字
        //        criteria.Add(SearchCriteria.CRITERIA_KEYWORD, "地震");

        //        //設定關鍵字(自動簡繁轉換)
        //        criteria.Add(SearchCriteria.CRITERIA_CHINESE_CONVERT, true);

        //        //設定來源
        //        criteria.Add(SearchCriteria.CRITERIA_DATA_SOURCE, "database");

        //        //排序條件使用精準度且按照降冪的方式排列
        //        order.SetOrderType("(1)", OrderType.ORDER_TYPE_DES);
        //        criteria.Add(SearchCriteria.CRITERIA_ORDER, order); //將排序條件加入至搜尋條件

        //        //將搜尋模式加入至搜尋條件(同義、同音詞模式搜尋)，SearchMode.SEARCH_MODE_SYNONYM 也搜尋不到
        //        criteria.Add(SearchCriteria.CRITERIA_SEARCH_MODE, SearchMode.SEARCH_MODE_FUZZY);

        //        //設定SearchService (SimpleCriteria, 0, 10) //(條件,起始位置,回傳最大的資料筆數)
        //        resultList = Searchservice.SearchBySimpleCriteria(criteria, 0, 100); //進階搜尋
        //        int Vsize = resultList.GetValidSize(); //取得回傳的筆數
        //        int Tsize = resultList.GetTotalSize(); //取得總筆數
        //        string Qtime = resultList.GetQueryTime(); //取回花費秒數

        //        //目前取回的值
        //        List<object> listResult = new List<object>();
        //        for (int i = 0; i < resultList.GetSize(); i++) {
        //            resultitem = resultList.GetItem(i);

        //            listResult.Add(new {
        //                keyWord = resultitem.GetKeywords(),     //取得關鍵字
        //                Summary = resultitem.GetContent(),      //取得內文摘要
        //            });
        //        }

        //        Console.Write(listResult.Count.ToString());     //0筆結果？

        //        return View();
        //    }
    }
}
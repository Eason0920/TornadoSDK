var actions = Reflux.createActions(['searchValueChange']);      //利用 reflux api 註冊監聽事件

//利用 reflux api 建立監聽事件處理儲存體
var searchStore = Reflux.createStore({
    searchResultJson: { search_results: [], search_mode_code: null, query_seconds: 0 },
    listenables: [actions],
    onSearchValueChange: function (searchValue, searchModeCode) {      //只要在儲存體定義的物件前面加上 on 即可對應到註冊的監聽事件
        var requestJson = {
            searchValue: searchValue,
            searchModeCode: searchModeCode
        };

        $.getJSON('/Home/getSearchResult', requestJson, function (response) {
            this.searchResultJson.search_results = response.search_results;
            this.searchResultJson.query_seconds = response.query_seconds;
            this.searchResultJson.search_mode_code = searchModeCode;
            this.trigger(this.searchResultJson);
        }.bind(this));
    }
});

//search bar component
var SearchBar = React.createClass({
    _searchButtonClick: function (event) {
        //this.setState({ searchValue: $('#inputSearch').val() });

        //發送 reflux 定義事件(按下搜尋按鈕)
        //if (this.refs.searchInput.value.length > 0) {
        //    actions.searchValueChange(this.refs.searchInput.value, this.props.searchModeCode);
        //}
        actions.searchValueChange(this.refs.searchInput.value, this.props.searchModeCode);
    },
    _onInputKeyUp: function (event) {
        if (event.keyCode === 13) {
            this._searchButtonClick();
        }
    },
    //inputChange: function (event) {
    //    this.setState({ searchValue: event.target.value });
    //},
    //getInitialState: function () {
    //    return { searchValue: '' };
    //},
    render: function () {
        return (
            <div>
                <div className="row">
                    <div className="col-md-12">
                        <div id="custom-search-input">
                            <label style={{ fontSize: '1.2em' }} htmlFor="inputSearch">{this.props.labelText}</label>
                            <div className="input-group">
                                <input id="inputSearch" type="text" className="form-control input-lg"
                                       placeholder="請輸入關鍵字 ..." ref="searchInput" onKeyUp={this._onInputKeyUp} />
                                <span className="input-group-btn">
                                    <button className="btn btn-info btn-lg" type="button" onClick={this._searchButtonClick}>
                                        <i className="glyphicon glyphicon-search"></i>
                                    </button>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
                {/*<ResultTable getSearchValue={this.state.searchValue} />*/}
            </div>
            )
    }
});

//search result table component
var ResultTable = React.createClass({
    mixins: [Reflux.listenTo(searchStore, '_onSearchResultChange')],        //訂閱儲存體發送的事件(使用 mixins 即可不需要手動解除訂閱活動)
    _onSearchResultChange: function (searchResultJson) {
        if (searchResultJson.search_mode_code == this.props.searchModeCode) {
            this.setState({
                aryResult: searchResultJson.search_results,
                resultCount: searchResultJson.search_results.length,
                querySeconds: searchResultJson.query_seconds
            });
        }
    },
    getInitialState: function () {
        return {
            aryResult: [],
            resultCount: 0,
            querySeconds: 0
        };
    },
    render: function () {
        return (
            <div>
                <div style={{ fontSize: '1.2em' }}>命中筆數： {this.state.resultCount}</div>
                <div style={{ fontSize: '1.2em', marginBottom: '5px' } }>花費秒數： {this.state.querySeconds}</div>
                <table className="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>hit</th>
                                    <th>[title]</th>
                                    <th>[desc]</th>
                                    {/**<th>[year]</th>
                                    <th>[date]</th>*/}
                                </tr>
                            </thead>
                            <tbody>
                                {this.state.aryResult.map(function (resultJson, idx) {
                                    return (
                                        <tr key={idx }>
                                            <td>{resultJson.id}</td>
                                            {/*html string to html dom*/}
                                            <td dangerouslySetInnerHTML={{ __html: resultJson.summary } }></td>
                                             <td dangerouslySetInnerHTML={{ __html: resultJson.title } }></td>
                                            <td dangerouslySetInnerHTML={{ __html: resultJson.description } }></td>
                                            {/**<td>{resultJson.year}</td>
                                             <td>{resultJson.create_date}</td>*/}
                                        </tr>
                                    )
                                })}
                            </tbody>
                </table>
            </div>
            )
    }
});

//ReactDOM.render(<SearchBar />, $('#searchBarContainer')[0]);
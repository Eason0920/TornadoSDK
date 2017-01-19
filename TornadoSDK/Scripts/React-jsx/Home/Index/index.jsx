ReactDOM.render(<SearchBar labelText="一般搜尋：" searchModeCode="0"/>, $('#searchBarContainer')[0]);
ReactDOM.render(<ResultTable searchModeCode="0" />, $('#searchResultContainer')[0]);
ReactDOM.render(<SearchBar labelText="模糊搜尋：" searchModeCode="1" />, $('#searchBarContainer1')[0]);
ReactDOM.render(<ResultTable searchModeCode="1" />, $('#searchResultContainer1')[0]);
ReactDOM.render(<SearchBar labelText="同義詞搜尋：" searchModeCode="2" />, $('#searchBarContainer2')[0]);
ReactDOM.render(<ResultTable searchModeCode="2" />, $('#searchResultContainer2')[0]);
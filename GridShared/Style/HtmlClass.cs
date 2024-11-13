namespace GridShared.Style
{
    public class HtmlClass
    {
        private readonly string[] _button = { "btn", "btn", "btn", "btn", "button" };
        private readonly string[] _buttonPrimary = { "btn btn-primary", "btn btn-primary", "btn btn-primary", "btn filled", "button is-link" };
        private readonly string[] _buttonSecondary = { "btn btn-secondary", "btn btn-secondary", "btn btn-secondary", "btn tonal", "button" };
        private readonly string[] _buttonLink = { "btn btn-link", "btn btn-link", "btn btn-link", "btn btn-small text", "button is-small" };
        private readonly string[] _card = { "panel panel-default", "card", "card", "card", "card" };
        private readonly string[] _cardBody = { "panel-body", "card-body", "card-body", "card-content", "card-content" };
        private readonly string[] _colMd = { "col-md-", "col-md-", "col-md-", "col m", "column is-" };
        private readonly string[] _dropdownMenu = { "dropdown dropdown-menu", "dropdown dropdown-menu", "dropdown dropdown-menu", "dropdown-content", "dropdown dropdown-menu" };
        private readonly string[] _footerRow = { "form-group row", "form-group row", "mb-3", "row mb-3", "columns" };
        private readonly string[] _formGroupRow = { "form-group row", "form-group row mb-3", "row mb-3", "row mb-3", "columns" };
        private readonly string[] _formLabel = { "control-label", "col-form-label", "col-form-label", "grid-materialize-label", "label" };
        private readonly string[] _formDivInput = { "", "", "", "input-field", "control" };
        private readonly string[] _formInput = { "form-control", "form-control", "form-control", "grid-materialize-input", "input" };
        private readonly string[] _formDivSelect = { "", "", "", "", "select grid-bulma-select" };
        private readonly string[] _formSelect = { "form-control", "form-control", "form-select", "grid-materialize-select", "grid-bulma-select" };
        private readonly string[] _formTextArea = { "form-control", "form-control", "form-control", "grid-materialize-textarea", "textarea" };
        private readonly string[] _formCheck = { "form-control", "form-check", "form-check", "", "checkbox" };
        private readonly string[] _formDivToggleSwitch = { "", "", "", "switch", "grid-bulma-switch" };
        private readonly string[] _formSpanToggleSwitch = { "grid-slider round", "grid-slider round", "grid-slider round", "lever", "grid-slider round" };
        private readonly string[] _inputGroup = { "input-group", "input-group", "input-group", "input-group", "field has-addons" };
        private readonly string[] _inputGroupLeftButton = { "input-group-btn", "input-group-btn", "input-group-btn", "prefix mt-2", "" };
        private readonly string[] _inputGroupRightButton = { "input-group-btn", "input-group-btn", "input-group-btn", "suffix mt-2", "" };
        private readonly string[] _listFilterValueLabel = { "", "", "", "my-5", "" };
        private readonly string[] _modal = { "modal", "modal", "modal", "modal", "modal" };
        private readonly string[] _modalDialog = { "modal-dialog", "modal-dialog", "modal-dialog", "modal-content", "" };
        private readonly string[] _modalContent = { "modal-content", "modal-content", "modal-content", "", "modal-content" };
        private readonly string[] _modalBody = { "modal-body", "modal-body", "modal-body", "", "modal-body" };
        private readonly string[] _mx000 = { "", "mx-0", "mx-0", "mx-0", "mx-0" };
        private readonly string[] _mx050 = { "", "mx-2", "mx-2", "mx-2", "mx-2" };
        private readonly string[] _mx100 = { "", "mx-3", "mx-3", "mx-4", "mx-4" };
        private readonly string[] _my000 = { "", "my-0", "my-0", "my-0", "my-0" };
        private readonly string[] _my050 = { "", "my-2", "my-2", "my-2", "my-2" };
        private readonly string[] _my100 = { "", "my-3", "my-3", "my-4", "my-4" };
        private readonly string[] _mt000 = { "", "mt-0", "mt-0", "mt-0", "mt-0" };
        private readonly string[] _mt050 = { "", "mt-2", "mt-2", "mt-2", "mt-2" };
        private readonly string[] _mt100 = { "", "mt-3", "mt-3", "mt-4", "mt-4" };
        private readonly string[] _mb000 = { "", "mb-0", "mb-0", "mb-0", "mb-0" };
        private readonly string[] _mb050 = { "", "mb-2", "mb-2", "mb-2", "mb-2" };
        private readonly string[] _mb100 = { "", "mb-3", "mb-3", "mb-4", "mb-4" };
        private readonly string[] _ml000 = { "", "ml-0", "ms-0", "ml-0", "ml-0" };
        private readonly string[] _ml050 = { "", "ml-2", "ms-2", "ml-2", "ml-2" };
        private readonly string[] _ml100 = { "", "ml-3", "ms-3", "ml-4", "ml-4" };
        private readonly string[] _mr000 = { "", "mr-0", "me-0", "mr-0", "mr-0" };
        private readonly string[] _mr050 = { "", "mr-2", "me-2", "mr-2", "mr-2" };
        private readonly string[] _mr100 = { "", "mr-3", "me-3", "mr-4", "mr-4" };
        private readonly string[] _offsetMd = { "col-md-offset-", "offset-md-", "offset-md-", "offset-m", "is-offset-" };
        private readonly string[] _paginationNav = { "", "", "", "", "pagination" };
        private readonly string[] _paginationList = { "pagination", "pagination", "pagination", "pagination", "pagination-list" };
        private readonly string[] _pageInput = { "form-control", "form-control", "form-control", "", "input" };
        private readonly string[] _pageItem = { "page-item", "page-item", "page-item", "", "" };
        private readonly string[] _pageItemActive = { "active", "active", "active", "active", "" };
        private readonly string[] _pageLink = { "page-link", "page-link", "page-link", "", "pagination-link" };
        private readonly string[] _pageLinkActive = { "", "", "", "", "is-current" };
        private readonly string[] _row = { "row", "row", "row", "row", "columns" };
        private readonly string[] _searchInput = { "form-control", "form-control", "form-control", "", "input" };
        private readonly string[] _tabDiv = { "", "", "", "", "tabs grid-tabs" };
        private readonly string[] _tabList = { "nav nav-tabs", "nav nav-tabs", "nav nav-tabs", "tabs", "" };
        private readonly string[] _tabItem = { "nav-item", "nav-item", "nav-item", "tab", "tab" };
        private readonly string[] _tabItemActive = { "active", "active", "active", "active", "is-active" };
        private readonly string[] _tabLink = { "nav-link", "nav-link", "nav-link", "", "" };
        private readonly string[] _tabLinkActive = { "", "active", "active", "active", "" };
        private readonly string[] _tabPaneHidden = { "", "", "", "hidden", "hidden" };
        private readonly string[] _tabPaneActive = { "active", "active", "active", "active", "active" };

        public CssFramework GridStyle { get; }

        public string Button { get { return _button[(int)GridStyle]; } }
        public string ButtonPrimary { get { return _buttonPrimary[(int)GridStyle]; } }
        public string ButtonSecondary { get { return _buttonSecondary[(int)GridStyle]; } }
        public string ButtonLink { get { return _buttonLink[(int)GridStyle]; } }
        public string Card { get { return _card[(int)GridStyle]; } }
        public string CardBody { get { return _cardBody[(int)GridStyle]; } }
        public string ColMd { get { return _colMd[(int)GridStyle]; } }
        public string DropdownMenu { get { return _dropdownMenu[(int)GridStyle]; } }
        public string FooterRow { get { return _footerRow[(int)GridStyle]; } }
        public string FormGroupRow { get { return _formGroupRow[(int)GridStyle]; } }
        public string FormLabel { get { return _formLabel[(int)GridStyle]; } }
        public string FormDivInput { get { return _formDivInput[(int)GridStyle]; } }
        public string FormInput { get { return _formInput[(int)GridStyle]; } }
        public string FormDivSelect { get { return _formDivSelect[(int)GridStyle]; } }
        public string FormSelect { get { return _formSelect[(int)GridStyle]; } }
        public string FormTextArea { get { return _formTextArea[(int)GridStyle]; } }
        public string FormCheck { get { return _formCheck[(int)GridStyle]; } }
        public string FormDivToggleSwitch { get { return _formDivToggleSwitch[(int)GridStyle]; } }
        public string FormSpanToggleSwitch { get { return _formSpanToggleSwitch[(int)GridStyle]; } }
        public string InputGroup { get { return _inputGroup[(int)GridStyle]; } }
        public string InputGroupLeftButton { get { return _inputGroupLeftButton[(int)GridStyle]; } }
        public string InputGroupRightButton { get { return _inputGroupRightButton[(int)GridStyle]; } }
        public string ListFilterValueLabel { get { return _listFilterValueLabel[(int)GridStyle]; } }
        public string Modal { get { return _modal[(int)GridStyle]; } }
        public string ModalDialog { get { return _modalDialog[(int)GridStyle]; } }
        public string ModalContent { get { return _modalContent[(int)GridStyle]; } }
        public string ModalBody { get { return _modalBody[(int)GridStyle]; } }
        public string Mx000 { get { return _mx000[(int)GridStyle]; } }
        public string Mx050 { get { return _mx050[(int)GridStyle]; } }
        public string Mx100 { get { return _mx100[(int)GridStyle]; } }
        public string My000 { get { return _my000[(int)GridStyle]; } }
        public string My050 { get { return _my050[(int)GridStyle]; } }
        public string My100 { get { return _my100[(int)GridStyle]; } }
        public string Mt000 { get { return _mt000[(int)GridStyle]; } }
        public string Mt050 { get { return _mt050[(int)GridStyle]; } }
        public string Mt100 { get { return _mt100[(int)GridStyle]; } }
        public string Mb000 { get { return _mb000[(int)GridStyle]; } }
        public string Mb050 { get { return _mb050[(int)GridStyle]; } }
        public string Mb100 { get { return _mb100[(int)GridStyle]; } }
        public string Ml000 { get { return _ml000[(int)GridStyle]; } }
        public string Ml050 { get { return _ml050[(int)GridStyle]; } }
        public string Ml100 { get { return _ml100[(int)GridStyle]; } }
        public string Mr000 { get { return _mr000[(int)GridStyle]; } }
        public string Mr050 { get { return _mr050[(int)GridStyle]; } }
        public string Mr100 { get { return _mr100[(int)GridStyle]; } }
        public string OffsetMd { get { return _offsetMd[(int)GridStyle]; } }
        public string PaginationNav { get { return _paginationNav[(int)GridStyle]; } }
        public string PaginationList { get { return _paginationList[(int)GridStyle]; } }
        public string PageInput { get { return _pageInput[(int)GridStyle]; } }
        public string PageItem { get { return _pageItem[(int)GridStyle]; } }
        public string PageItemActive { get { return _pageItemActive[(int)GridStyle]; } }
        public string PageLink { get { return _pageLink[(int)GridStyle]; } }
        public string PageLinkActive { get { return _pageLinkActive[(int)GridStyle]; } }
        public string Row { get { return _row[(int)GridStyle]; } }
        public string SearchInput { get { return _searchInput[(int)GridStyle]; } }
        public string TabDiv { get { return _tabDiv[(int)GridStyle]; } }
        public string TabList { get { return _tabList[(int)GridStyle]; } }
        public string TabItem { get { return _tabItem[(int)GridStyle]; } }
        public string TabItemActive { get { return _tabItemActive[(int)GridStyle]; } }
        public string TabLink { get { return _tabLink[(int)GridStyle]; } }
        public string TabLinkActive { get { return _tabLinkActive[(int)GridStyle]; } }
        public string TabPaneActive { get { return _tabPaneActive[(int)GridStyle]; } }
        public string TabPaneHidden { get { return _tabPaneHidden[(int)GridStyle]; } }

        public HtmlClass()
        {
            GridStyle = CssFramework.Bootstrap_4;
        }

        public HtmlClass(CssFramework gridStyle) 
        { 
            GridStyle = gridStyle;
        }
    }
}

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Demo.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <title>Access Control List | Test Harness</title>

    <link href="/Styles/bootstrap.min.css" rel="stylesheet">
    <link href="/Fonts/font-awesome/css/font-awesome.css" rel="stylesheet">

    <link href="/Styles/animate.css" rel="stylesheet">
    <link href="/Styles/style.css" rel="stylesheet">

</head>
<body>
    
    <form runat="server">
        
    <div id="wrapper">
    
        <div id="page-wrapper" class="gray-bg">
            
            <div class="row wrapper border-bottom white-bg page-heading">
                <div class="col-lg-12">
                    <h1>Access Control List</h1>
                    <ol class="breadcrumb">
                        <li>
                            Simple Test Harness
                        </li>
                    </ol>
                </div>
            </div>
            

            <div class="wrapper wrapper-content animated fadeInRight">
                    
                <!-- This seems to be required to get the bootstrap forms to render properly (no idea why) -->
                <div class="form-horizontal"></div>
                
                <div runat="server" ID="StatusMessage" Visible="false" class="alert alert-danger" EnableViewState="False"></div>

                <div class="row">
                    
                    <div class="col-lg-4">
                        
                        <div class="ibox float-e-margins">
                            <div class="ibox-title">
                                <h5>Load Access Control List</h5>
                            </div>
                            <div class="ibox-content">
                                <p>
                                    Create an access control list and load it with permissions from a CSV text file.
                                </p>
                                <asp:FileUpload runat="server" ID="UploadCsv" />
                                    
                            </div>
                            <div class="ibox-content">
                                <asp:Button runat="server" ID="LoadGrantedFromCsv" Text="Load Permissions Granted" CssClass="btn btn-w-m btn-primary" />
                                <asp:Button runat="server" ID="LoadDeniedFromCsv" Text="Load Permissions Denied" CssClass="btn btn-w-m btn-danger" />
                                <asp:Button runat="server" ID="ResetGrantedAndDenied" Text="Reset" CssClass="btn btn-w-m btn-default" />
                            </div>
                            <div class="ibox-footer" runat="server" ID="LoadFooter">
                                <span class="pull-right">
                                    
                                </span>
                                <asp:Literal runat="server" ID="TableRowCount" /> rows in data table (approx <asp:Literal runat="server" ID="TableSize" /> KB)
                            </div>
                        </div>
                            
                    </div>
                        
                    <div class="col-lg-4">
                            
                        <div class="ibox float-e-margins">
                            <div class="ibox-title">
                                <h5>Grant, Deny, or Revoke Individual Permission</h5>
                            </div>
                            <div class="ibox-content">
                                <p>
                                    Explicitly grant or deny a specific permission.
                                </p>
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <label class="col-lg-2 control-label">Principal</label>
                                        <div class="col-lg-10">
                                            <asp:TextBox runat="server" ID="AclPrincipal" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-2 control-label">Operation</label>
                                        <div class="col-lg-10">
                                            <asp:TextBox runat="server" ID="AclOperation" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-2 control-label">Resource</label>
                                        <div class="col-lg-10">
                                            <asp:TextBox runat="server" ID="AclResource" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="ibox-footer">
                                <asp:Button runat="server" ID="GrantPermission" Text="Grant" CssClass="btn btn-w-m btn-primary" />
                                <asp:Button runat="server" ID="RevokePermission" Text="Revoke" CssClass="btn btn-w-m btn-default" />
                                <asp:Button runat="server" ID="DenyPermission" Text="Deny" CssClass="btn btn-w-m btn-danger" />
                            </div>
                        </div>

                    </div>
                        
                    <div class="col-lg-4">

                        <div class="ibox float-e-margins">
                            <div class="ibox-title">
                                <h5>Check Permission</h5>
                            </div>
                            <div class="ibox-content">
                                <p>
                                    Check to see if any one of a user's roles have been granted a specific permission
                                </p>
                                <div class="form-horizontal">
                                    <div class="form-group">
                                        <label class="col-lg-2 control-label">Principals</label>
                                        <div class="col-lg-10">
                                            <asp:TextBox runat="server" ID="CheckRoles" CssClass="form-control" />
                                            <span class="help-block m-b-none">Enter a comma-separated list of user roles here</span>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-2 control-label">Operation</label>
                                        <div class="col-lg-10">
                                            <asp:TextBox runat="server" ID="CheckOperation" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-lg-2 control-label">Resource</label>
                                        <div class="col-lg-10">
                                            <asp:TextBox runat="server" ID="CheckResource" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="ibox-footer">
                                <asp:Button runat="server" ID="CheckPermission" Text="Check" CssClass="btn btn-w-m btn-success" />
                            </div>
                        </div>

                    </div>

                    <div class="col-lg-12">
                        
                        <div class="ibox float-e-margins" runat="server" ID="GrantedPanel">
                            <div class="ibox-title">
                                <h5>Permissions Granted</h5>
                            </div>
                            <div class="ibox-content">
                                <asp:GridView runat="server" ID="GrantedGrid" AutoGenerateColumns="True" CssClass="table table-striped">
                                    
                                </asp:GridView>
                            </div>
                        </div>
                        
                        <div class="ibox float-e-margins" runat="server" ID="DeniedPanel">
                            <div class="ibox-title">
                                <h5>Permissions Denied</h5>
                            </div>
                            <div class="ibox-content">
                                <asp:GridView runat="server" ID="DeniedGrid" AutoGenerateColumns="True" CssClass="table table-striped">
                                    
                                </asp:GridView>
                            </div>
                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

    </form>
    
    <ul class="nav metismenu" id="side-menu"></ul>

    <!-- Scripts -->
    <script src="/Scripts/jquery-2.1.1.js"></script>
    <script src="/Scripts/bootstrap.min.js"></script>
    <script src="/Scripts/plugins/metisMenu/jquery.metisMenu.js"></script>
    <script src="/Scripts/inspinia.js"></script>

</body>
</html>

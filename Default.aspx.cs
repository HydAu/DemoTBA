using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Demo
{
    public partial class Default : Page
    {
        private const int MaximumRowCount = 500;

        private DataTable Granted
        {
            get
            {
                object value = Session["Granted"] as DataTable;
                if (value == null)
                    Session["Granted"] = CreateTable();
                return (DataTable)Session["Granted"];
            }
            set { Session["Granted"] = value; }
        }

        private DataTable Denied
        {
            get
            {
                object value = Session["Denied"] as DataTable;
                if (value == null)
                    Session["Denied"] = CreateTable();
                return (DataTable)Session["Denied"];
            }
            set { Session["Denied"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            LoadGrantedFromCsv.Click += LoadGrantedFromCsv_Click;
            LoadDeniedFromCsv.Click += LoadDeniedFromCsv_Click;
            ResetGrantedAndDenied.Click += Reset_Click;

            GrantPermission.Click += GrantPermission_Click;
            RevokePermission.Click += RevokePermission_Click;
            DenyPermission.Click += DenyPermission_Click;
            CheckPermission.Click += CheckPermission_Click;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BindPermissions();
        }

        void AddNewRow(DataTable table)
        {
            var row = table.NewRow();
            row["Principal"] = AclPrincipal.Text;
            row["Operation"] = AclOperation.Text;
            row["Resource"] = AclResource.Text;
            table.Rows.Add(row);
        }

        private void BindPermissions()
        {
            var grantCount = Granted.Rows.Count;
            var denyCount = Denied.Rows.Count;

            LoadFooter.Visible = grantCount > 0;
            LoadGrantedFromCsv.Enabled = grantCount == 0;
            LoadDeniedFromCsv.Enabled = grantCount > 0 && denyCount == 0;
            ResetGrantedAndDenied.Enabled = grantCount > 0;
            CheckPermission.Enabled = grantCount > 0;

            GrantedPanel.Visible = grantCount > 0;
            DeniedPanel.Visible = denyCount > 0;

            if (grantCount > 0)
            {
                TableRowCount.Text = string.Format("{0:n0}", grantCount);
                TableSize.Text = string.Format("{0:n0}", Granted.SizeInBytes()/1024);
                BindPermissionsGrid(GrantedGrid, Granted);
            }

            if (denyCount > 0)
                BindPermissionsGrid(DeniedGrid, Denied);
        }

        private static void BindPermissionsGrid(GridView grid, DataTable table)
        {
            if (table != null)
            {
                grid.BorderWidth = 0;
                grid.BorderStyle = BorderStyle.None;
                grid.GridLines = GridLines.None;

                grid.DataSource = table;
                grid.DataBind();

                if (table.Rows.Count > 0)
                    grid.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        void CheckPermission_Click(object sender, EventArgs e)
        {
            IAccessControlList acl = new AccessControlList();
            AccessControlListAdapter.Load(acl, Granted, Denied);

            var roles = CheckRoles.Text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            if (acl.IsDenied(roles, CheckOperation.Text, CheckResource.Text))
            {
                StatusMessage.InnerText = "Access Denied: " + acl.Explain(roles, CheckOperation.Text, CheckResource.Text);
                StatusMessage.Attributes["class"] = "alert alert-danger";
            }
            else if (acl.IsGranted(roles, CheckOperation.Text, CheckResource.Text))
            {
                StatusMessage.InnerText = "Access Granted: " + acl.Explain(roles, CheckOperation.Text, CheckResource.Text);
                StatusMessage.Attributes["class"] = "alert alert-success";
            }
            else
            {
                StatusMessage.InnerText = string.Format( "Access Denied: Permission is not granted to any of the user roles in this set ({2}) for operation {0} on resource {1}", CheckOperation.Text, CheckResource.Text, CheckRoles.Text);
                StatusMessage.Attributes["class"] = "alert alert-warning";
            }
            
            StatusMessage.Visible = true;
        }

        static DataTable CreateTable()
        {
            var table = new DataTable();
            table.Columns.Add("Principal");
            table.Columns.Add("Operation");
            table.Columns.Add("Resource");
            table.Columns.Add("Comment");
            return table;
        }

        private static UploadModel CreateTableFromCsvFile(FileUpload upload)
        {
            var model = new UploadModel();

            if (!upload.HasFile)
            {
                model.Error = "Please select a file to upload.";
                model.Table = null;
            }
            else
            {
                var csv = new CsvHelper();
                csv.Read(upload.PostedFile.InputStream, Encoding.UTF8);

                if (!csv.Table.Columns.Contains("Principal"))
                    model.Error = "Missing Column: Principal";

                if (!csv.Table.Columns.Contains("Operation"))
                    model.Error = "Missing Column: Operation";

                if (!csv.Table.Columns.Contains("Resource"))
                    model.Error = "Missing Column: Resource";

                if (csv.Table.Rows.Count > MaximumRowCount)
                    model.Error = string.Format("This test harness is intended for small data sets only. Please limit the size of your file to {0} rows or less.", MaximumRowCount);

                model.Table = csv.Table;
            }

            return model;
        }

        void DenyPermission_Click(object sender, EventArgs e)
        {
            AddNewRow(Denied);
            BindPermissions();
        }

        void GrantPermission_Click(object sender, EventArgs e)
        {
            AddNewRow(Granted);
            BindPermissions();
        }

        private void LoadDeniedFromCsv_Click(object sender, EventArgs e)
        {
            var model = CreateTableFromCsvFile(UploadCsv);
            if (model.Table != null)
            {
                Denied = model.Table;
                BindPermissions();
            }
            else
            {
                StatusMessage.InnerHtml = model.Error;
                StatusMessage.Visible = true;
            }
        }

        private void LoadGrantedFromCsv_Click(object sender, EventArgs e)
        {
            var model = CreateTableFromCsvFile(UploadCsv);
            if (model.Table != null)
            {
                Granted = model.Table;
                BindPermissions();
            }
            else
            {
                StatusMessage.InnerHtml = model.Error;
                StatusMessage.Visible = true;
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            Granted = null;
            Denied = null;
            BindPermissions();
        }

        void RevokePermission_Click(object sender, EventArgs e)
        {
            var p = StringHelper.Quote(AclPrincipal.Text);
            var o = StringHelper.Quote(AclOperation.Text);
            var r = StringHelper.Quote(AclResource.Text);
            var filter = string.Format("Principal = '{0}' AND Operation = '{1}' AND Resource = '{2}'", p, o, r);
            
            var rows = Granted.Select(filter);
            foreach (var row in rows)
                row.Delete();
            Granted.AcceptChanges();

            rows = Denied.Select(filter);
            foreach (var row in rows)
                row.Delete();
            Denied.AcceptChanges();

            BindPermissions();
        }
    }
}
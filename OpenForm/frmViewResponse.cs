using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenForm
{
    public partial class frmViewResponse : Form
    {
        DataRow source;
        DataColumnCollection header;
        DataTable presenter;

        public frmViewResponse(DataRow source, DataColumnCollection header)
        {
            InitializeComponent();
            this.source = source;
            this.header = header;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmViewResponse_Load(object sender, EventArgs e)
        {
            presenter = new DataTable();
            presenter.Columns.Add("Field");
            presenter.Columns.Add("Value");
            foreach(DataColumn c in header)
            {
                DataRow r = presenter.NewRow();
                r["Field"] = c.Caption;
                r["Value"] = source[c];
                presenter.Rows.Add(r);
            }
            dataGridView1.DataSource = presenter;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

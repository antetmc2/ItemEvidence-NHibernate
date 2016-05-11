using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ItemEvidence.DAL;
using ItemEvidence.Model;

namespace ItemEvidence.PresentationLayer
{
    public partial class EmployeeAssignedItems : Form, IObserver
    {
        private IMainController _controller;
        private ItemRepositoryMemory _itemRepo;
        private Employee _employee;
        private IList<EmployeeItemsList> _list = new List<EmployeeItemsList>();
        public EmployeeAssignedItems(IMainController controller, ItemRepositoryMemory itemRepo, Employee employee, IList<EmployeeItemsList> list)
        {
            _controller = controller;
            _itemRepo = itemRepo;
            _employee = employee;
            _list = list;
            InitializeComponent();
        }

        private void RefreshView()
        {
            LoadData();
            dataGridView_Items.Refresh();
            dataGridView_Items.Update();
            itemBindingSource.ResetBindings(true);
        }

        private void LoadData()
        {
            textBox_FirstName.Text = _employee.FirstName;
            textBox_LastName.Text = _employee.LastName;
            textBox_NumberOfItem.Text = 0.ToString();
            numericUpDown_Assign.Value = 0;
            numericUpDown_Take.Value = 0;

            itemBindingSource.Clear();
            Item test;
            foreach(var item in _employee.Items/*.OrderBy(x => x.Item.Name)*/)
            {
                try
                {
                    test = _itemRepo.Get(item.Item.ItemId);
                    itemBindingSource.Add(test);
                }
                catch { }
            }
            dataGridView_Items.AutoGenerateColumns = false;
            dataGridView_Items.AutoSize = true;
            dataGridView_Items.DataSource = itemBindingSource;
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EmployeeAssignedItems_Load(object sender, EventArgs e)
        {
            btn_AssignMore.Enabled = false;
            btn_Take.Enabled = false;
            btn_TakeAll.Enabled = false;
            numericUpDown_Assign.ReadOnly = true;
            numericUpDown_Take.ReadOnly = true;
            LoadData();
        }

        private void dataGridView_Items_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            var item = itemBindingSource.Current as Item;
            textBox_NumberOfItem.Text = _employee.GetItem(item).NumberAssigned.ToString();
            btn_AssignMore.Enabled = true;
            btn_Take.Enabled = true;
            btn_TakeAll.Enabled = true;
            numericUpDown_Assign.ReadOnly = false;
            numericUpDown_Take.ReadOnly = false;
        }

        private void btn_AssignMore_Click(object sender, EventArgs e)
        {
            Item item = itemBindingSource.Current as Item;
            _controller.AssignItemToEmployee(item.ItemId, _employee.EmpId, Convert.ToInt32(numericUpDown_Assign.Value));
        }

        private void btn_Take_Click(object sender, EventArgs e)
        {
            Item item = itemBindingSource.Current as Item;
            _controller.TakeItemFromEmployee(item.ItemId, _employee.EmpId, Convert.ToInt32(numericUpDown_Take.Value));
        }

        private void btn_TakeAll_Click(object sender, EventArgs e)
        {
            Item item = itemBindingSource.Current as Item;
            _controller.TakeItemFromEmployee(item.ItemId, _employee.EmpId);
        }

        public void UpdateView()
        {
            _employee = _itemRepo.GetEmployee(_employee.EmpId);
            RefreshView();
        }
    }
}

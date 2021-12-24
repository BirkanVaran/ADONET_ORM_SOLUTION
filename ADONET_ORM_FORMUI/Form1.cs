using ADONET_ORM_BLL;
using ADONET_ORM_Entites.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADONET_ORM_FORMUI
{
    public partial class FormCategories : Form
    {
        public FormCategories()
        {
            InitializeComponent();
        }
        // GLOBAL ALAN
        List<Category> categoryList = new List<Category>();
        CategoriesORM myCategoriesORM = new CategoriesORM();
        private void FormCategories_Load(object sender, EventArgs e)
        {
            TumKategorileriGrideDoldur();
        }
        private void TumKategorileriGrideDoldur()
        {
            categoryList = myCategoriesORM.Select();
            dataGridViewCategories.DataSource = categoryList;
        }
    }
}

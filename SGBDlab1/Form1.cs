using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace SGBDlab1
{
	public partial class Form1 : Form
	{
		SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
		SqlDataAdapter adapter = new SqlDataAdapter();
		DataSet data = new DataSet();
		BindingSource bs = new BindingSource();
		SqlDataAdapter adapter2 = new SqlDataAdapter();
		DataSet data2 = new DataSet();
		BindingSource bs2 = new BindingSource();
		string ChildTableName = ConfigurationManager.AppSettings["ChildTableName"];
		string ChildColumnNames = ConfigurationManager.AppSettings["ChildColumnNames"];

		public Form1()
		{
			InitializeComponent();
		}

		// Add button
		private void button1_Click(object sender, EventArgs e)
		{
			string conn2 = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
			SqlConnection conn = new SqlConnection(conn2);
			try
			{
				string ColumnNamesInsertParameters = ConfigurationManager.AppSettings["ColumnNamesInsertParameters"];
				List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));
				adapter.InsertCommand = new SqlCommand("INSERT INTO " + ChildTableName + " VALUES(" + ColumnNamesInsertParameters + ")", conn);
				foreach (string column in ColumnNamesList)
				{
					TextBox textBox = (TextBox)panel1.Controls[column];
					adapter.InsertCommand.Parameters.AddWithValue("@" + column, textBox.Text);
				}
				conn.Open();
				adapter.InsertCommand.ExecuteNonQuery();
				MessageBox.Show("Inserted Succesfull to the Database");
				conn.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				conn.Close();
			}
		}

		// Update button
		private void button3_Click(object sender, EventArgs e)
		{
			string conn2 = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
			SqlConnection conn = new SqlConnection(conn2);
			int x;
			string UpdateQuery = ConfigurationSettings.AppSettings["UpdateQuery"];
			adapter.UpdateCommand = new SqlCommand(UpdateQuery, conn);
			List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));
			foreach (string column in ColumnNamesList)
			{
				TextBox textBox = (TextBox)panel1.Controls[column];
				adapter.UpdateCommand.Parameters.AddWithValue("@" + column, textBox.Text);
			}
			conn.Open();
			x = adapter.UpdateCommand.ExecuteNonQuery();
			conn.Close();
			if (x >= 1)
			{
				MessageBox.Show("The record has been updated");
			}
		}

		// Delete button
		private void button2_Click(object sender, EventArgs e)
		{
			string conn2 = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
			SqlConnection conn = new SqlConnection(conn2);
			DialogResult dr;
			dr = MessageBox.Show("Are you sure?\n No undo after delete", "Confirm Deletion", MessageBoxButtons.YesNo);
			if (dr == DialogResult.Yes)
			{
				string delete = ConfigurationSettings.AppSettings["delete"];
				adapter.DeleteCommand = new SqlCommand(delete, conn);
				List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));
				TextBox textBox = (TextBox)panel1.Controls[ColumnNamesList[0]];
				adapter.DeleteCommand.Parameters.AddWithValue("@" + ColumnNamesList[0], textBox.Text);
				conn.Open();
				adapter.DeleteCommand.ExecuteNonQuery();
				conn.Close();
				data2.Clear();
				adapter.Fill(data2);
			}
			else
			{
				MessageBox.Show("Deletion Aborded");
			}
		}

		// Refresh button
		private void button4_Click(object sender, EventArgs e)
		{
			string select = ConfigurationSettings.AppSettings["select"];
			adapter.SelectCommand = new SqlCommand(select, conn);
			data.Clear();
			adapter.Fill(data);
			dataGridView1.DataSource = data.Tables[0];
			bs.DataSource = data.Tables[0];
			string select2 = ConfigurationSettings.AppSettings["selectChild"];
			adapter2.SelectCommand = new SqlCommand(select2, conn);
			adapter2.SelectCommand.Parameters.Add(ConfigurationManager.AppSettings["legatura"], SqlDbType.SmallInt).Value = 1;
			data2.Clear();
			adapter2.Fill(data2);
			dataGridView2.DataSource = data2.Tables[0];
			bs2.DataSource = data2.Tables[0];
			GenerateTexts();
		}

		private void DataGridViewUpdate()
		{
			string conn2 = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
			SqlConnection conn = new SqlConnection(conn2);
			dataGridView1.ClearSelection();
			dataGridView1.Rows[bs.Position].Selected = true;
			dataGridView2.ClearSelection();
			dataGridView2.Rows[bs2.Position].Selected = true;
			string select2 = ConfigurationSettings.AppSettings["selectChild"];
			adapter2.SelectCommand = new SqlCommand(select2, conn);
			adapter2.SelectCommand.Parameters.Add(ConfigurationManager.AppSettings["legatura"], SqlDbType.SmallInt).Value = Int32.Parse(panel1.Controls[ConfigurationManager.AppSettings["legatura"]].Text);
			data2.Clear();
			adapter2.Fill(data2);
			dataGridView2.DataSource = data2.Tables[0];
			bs2.DataSource = data2.Tables[0];
		}

		// Next button
		private void button5_Click(object sender, EventArgs e)
		{
			bs.MoveNext();
			DataGridViewUpdate();
		}

		// Previous button
		private void button6_Click(object sender, EventArgs e)
		{
			bs.MovePrevious();
			DataGridViewUpdate();
		}

		private void GenerateTexts()
		{
			int cLeft = 10;
			List<string> ColumnNamesList = new List<string>(ConfigurationManager.AppSettings["ChildColumnNames"].Split(','));
			try
			{
				foreach (string column in ColumnNamesList)
				{
					TextBox textBox = AddNewTextBox(cLeft, column);
					AddNewLabel(cLeft, column);
					cLeft += 5;
					if (column == ConfigurationManager.AppSettings["legatura"])
					{
						textBox.DataBindings.Add("Text", bs, column);
					}
					else
					{
						textBox.DataBindings.Add("Text", bs2, column);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				conn.Close();
			}
		}

		public TextBox AddNewTextBox(int cleft, string text)
		{
			TextBox txt = new TextBox();
			txt.Name = text;
			panel1.Controls.Add(txt);
			txt.Top = cleft * 10;
			txt.Left = 100;
			txt.MaxLength = 30;
			txt.Text = "TextBox" + text;
			txt.Size = new System.Drawing.Size(142, 27);
			return txt;
		}

		public Label AddNewLabel(int cleft, string text)
		{
			Label label = new Label();
			panel1.Controls.Add(label);
			label.Top = cleft * 10;
			label.Left = 15;
			label.Text = text;
			return label;
		}
	}
}

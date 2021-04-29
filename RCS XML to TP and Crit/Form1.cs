using System;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RCS_XML_to_TP_and_Crit
{
    public partial class Form1 : Form
    {
        StreamWriter TestPlan;
        StreamWriter Criteria;

        string plan = @"C:\Temp\TestPlan.txt";
        string criteria = @"C:\Temp\Criteria.txt";
        public Form1()
        {
            InitializeComponent();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loadFLXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateFiles();
        }

        private void GenerateFiles()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "FL XML file (*.xml)|*.xml";
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileDialog.FileName);
                XmlNodeReader nodeReader = new XmlNodeReader(doc);
                TestPlan = new StreamWriter(plan);
                Criteria = new StreamWriter(criteria);
                TestPlan.WriteLine("GROUP(TESTBOX) BEGIN");
                //probably add here some more lines like testbox log enable
                Criteria.WriteLine("<MPG> TESTBOX_RCS, \"Auto generated Criteria code from RCS .xml file\"");
                Criteria.WriteLine("<DATA> TIMEOUT_I, 180000");
                while (nodeReader.Read())
                {
                    int nodes = nodeReader.AttributeCount;
                    if (nodeReader.IsStartElement() && nodeReader.Name.Equals("include"))
                    {
                        string messageInclude = $"There's include file {nodeReader.ReadInnerXml()}\nDownload it and hit Yes to load it.\nTo skip - hit No.";
                        string captionInclude = "Message!";
                        MessageBoxButtons buttonsInclude = MessageBoxButtons.YesNo;
                        DialogResult resultInclude;
                        resultInclude = MessageBox.Show(messageInclude, captionInclude, buttonsInclude);
                        if (resultInclude == System.Windows.Forms.DialogResult.Yes)
                        {
                            GenerateExtra();
                        }
                    }
                    if (nodeReader.IsStartElement() && nodeReader.Name.Equals("testset"))
                    {
                        TestPlan.WriteLine($"  RUN({nodeReader.GetAttribute(0).ToUpper()}, \"{nodeReader.GetAttribute(1)} <{nodeReader.GetAttribute(0).ToUpper()}>, mTestBox, DUT1:TESTBOX_RCS:{nodeReader.GetAttribute(0).ToUpper()},{new Func<string>(() => { if (String.IsNullOrEmpty(textBox1.Text)) return ""; else return $" CONNECTION_NAME_S:{textBox1.Text};".ToUpper(); }).Invoke()},)");
                        Criteria.WriteLine($"<MP:I> {nodeReader.GetAttribute(0).ToUpper()},\"{nodeReader.GetAttribute(1)} {new Func<string>(() => { if (nodes >= 4) return $"({ nodeReader.GetAttribute(3)})"; else return ""; }).Invoke()}\",,,EQU,1");
                        Criteria.WriteLine($"<DATA> TBCOMMAND_S,\"{nodeReader.GetAttribute(0)}\"{new Func<string>(() => { if (nodeReader.GetAttribute(0) == "cpm_0011") return $",,SEARCH_AS,cores:{textBox2.Text.ToString()}"; else if (nodeReader.GetAttribute(0) == "early_init") return $",,SEARCH_AS,{textBox3.Text.ToString()}"; else return ""; }).Invoke()}");
                    }
                }
                string message = "No more includes\nAny other files to load?";
                string caption = "Message!";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                    GenerateExtra();
                TestPlan.WriteLine("END_GROUP");
                TestPlan.Close();
                Criteria.Close();
            }
        }

        private void GenerateExtra()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "FL XML file (*.xml)|*.xml";
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileDialog.FileName);
                XmlNodeReader nodeReader = new XmlNodeReader(doc);
                while (nodeReader.Read())
                {
                    int nodes = nodeReader.AttributeCount;
                    if (nodeReader.IsStartElement() && nodeReader.Name.Equals("include"))
                    {
                        string messageInclude = $"There's include file {nodeReader.ReadInnerXml()}\nDownload it and hit Yes to load it.\nTo skip - hit No.";
                        string captionInclude = "Message!";
                        MessageBoxButtons buttonsInclude = MessageBoxButtons.YesNo;
                        DialogResult resultInclude;
                        resultInclude = MessageBox.Show(messageInclude, captionInclude, buttonsInclude);
                        if (resultInclude == System.Windows.Forms.DialogResult.Yes)
                        {
                            GenerateExtra();
                        }
                    }
                    if (nodeReader.IsStartElement() && nodeReader.Name.Equals("testset"))
                    {
                        TestPlan.WriteLine($"  RUN({nodeReader.GetAttribute(0).ToUpper()}, \"{nodeReader.GetAttribute(1)} <{nodeReader.GetAttribute(0).ToUpper()}>, mTestBox, DUT1:TESTBOX_RCS:{nodeReader.GetAttribute(0).ToUpper()},{new Func<string>(() => { if (String.IsNullOrEmpty(textBox1.Text)) return ""; else return $" CONNECTION_NAME_S:{textBox1.Text};".ToUpper(); }).Invoke()},)");
                        Criteria.WriteLine($"<MP:I> {nodeReader.GetAttribute(0).ToUpper()},\"{nodeReader.GetAttribute(1)} {new Func<string>(() => { if (nodes >= 4) return $"({ nodeReader.GetAttribute(3)})"; else return ""; }).Invoke()}\",,,EQU,1");
                        Criteria.WriteLine($"<DATA> TBCOMMAND_S,\"{nodeReader.GetAttribute(0)}\"{new Func<string>(() => { if (nodeReader.GetAttribute(0) == "cpm_0011") return $",,SEARCH_AS,cores:{textBox2.Text.ToString()}"; else if (nodeReader.GetAttribute(0) == "early_init") return $",,SEARCH_AS,{textBox3.Text.ToString()}"; else return ""; }).Invoke()}");
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://bi-bb-ee.sero.wh.rnd.internal.ericsson.com/opengrok/xref/rcs-lls/hwtest/config/");
        }
    }
}

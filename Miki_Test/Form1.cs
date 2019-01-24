using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;





namespace Miki_Test
{
    public partial class Form1 : Form
    {

        string fFilter1;
        string fFilter2;
        string fFilter3;
        public string gPath { get; set;}

        public Form1()
        {            
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {           

            //get a list of the drives
            //string[] drives = Environment.GetLogicalDrives();
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
               // Console.WriteLine(drive);
                //DriveInfo di = new DriveInfo(drive);
                int driveImage;

                switch (drive.DriveType)    //set the drive's icon
                {
                    case DriveType.CDRom:
                        driveImage = 3;
                        break;
                    case DriveType.Network:
                        driveImage = 6;
                        break;
                    case DriveType.NoRootDirectory:
                        driveImage = 8;
                        break;
                    case DriveType.Unknown:
                        driveImage = 8;
                        break;
                    default:
                        driveImage = 2;
                        break;
                }

                TreeNode node = new TreeNode(drive.Name.Substring(0, 1), driveImage, driveImage);
                node.Tag = drive;

                if (drive.IsReady == true)
                   node.Nodes.Add("...");

                treeView1.Nodes.Add(node);
            }
        }

        private void dirsTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            
            
            if (e.Node.Nodes.Count > 0)
            {
                //if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
                //{
                    e.Node.Nodes.Clear();
                    //get the list of sub direcotires
                    string[] files;
                    string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString());

                    if (radioButton1.Checked ^ radioButton2.Checked ^ radioButton3.Checked)
                    {                        
                        files = Directory.GetFiles(e.Node.Tag.ToString(), fFilter1 );                      
                    }
                    else
                    {
                        files = Directory.GetFiles(e.Node.Tag.ToString());                        
                    }




                        //string[] files = Directory.GetFiles(e.Node.Tag.ToString());

                    foreach (string fil in files)
                    {
                        DirectoryInfo fi = new DirectoryInfo(fil);
                        TreeNode node = new TreeNode(fi.Name, 0, 1);
                        try
                        {
                            node.Tag = fil;
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);

                        }

                    } 
                       
                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        TreeNode node = new TreeNode(di.Name, 0, 1);

                        try
                        {
                            //keep the directory's full path in the tag for use later
                            node.Tag = dir;

                            //if the directory has sub directories add the place holder
                            if (di.GetDirectories().Count() > 0)
                                node.Nodes.Add(null, "...", 0, 0);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //display a locked folder icon
                            node.ImageIndex = 12;
                            node.SelectedImageIndex = 12;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryLister",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }
                    }
                //}
            }

        }



        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {


            string text = treeView1.SelectedNode.FullPath;
            string temp = text[0] + ":\\";//Ime drajva           
            char[] tempFullPath = text.ToCharArray();
            char[] tempr = new char[text.Length];

            for (int i = 2; i < text.Length; i++)
            {
                tempr[i - 2] = text[i];
            }

            string mFullPath = new string(tempr);
            gPath = string.Empty;
            gPath = temp + mFullPath;
            label9.Text = temp + mFullPath;//Show FullPath 
            //gPath = label9.Text;
            //label22.Text = gPath;
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            try
            {
                foreach (DriveInfo drive in allDrives)
                {
                    string driveName = drive.Name;

                    if (driveName[0] == temp[0])
                    {
                        label1.Text = drive.Name;
                        label2.Text = drive.VolumeLabel;
                        try
                        {
                            label4.Text = drive.TotalSize.ToString() + " bytes";
                            label5.Text = drive.AvailableFreeSpace.ToString() + "bytes";
                        }
                        catch (IOException)
                        {
                        }
                    }

                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(label9.Text);
                        int fileCounter = 0;

                        foreach (var i in di.GetFiles())
                        {
                            fileCounter++;
                        }

                        label7.Text = fileCounter.ToString();

                        if (treeView1.SelectedNode.Text == di.Name)
                            label3.Text = di.Name;//Show folder name
                        else
                            label3.Text = "-";
                        label8.Text = di.Attributes.ToString();
                    }
                    catch (IOException)
                    {
                        label3.Text = "-";
                        label8.Text = "-";
                        label7.Text = "-";
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }

                    try
                    {
                        FileInfo fi = new FileInfo(label9.Text);
                        label6.Text = fi.Name;
                        label21.Text = fi.Length.ToString() + " bytes";
                        label20.Text = fi.Attributes.ToString();
                    }
                    catch (Exception)
                    {
                        label6.Text = "-";
                        label21.Text = "-";
                        label20.Text = "-";
                    }

                }
            }
            catch (IOException)
            {

            }
        }
    
        
             

       
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Node.Text.Contains(""))
                    System.Diagnostics.Process.Start(gPath + e.Node.ToString());
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("File not faund");
            }
           
        }





       




        private void ddirsTreeView_BeforeExpand(object sender, TreeViewEventArgs e)
        {           
            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
                {
                    e.Node.Nodes.Clear();

                    //get the list of sub direcotires
                    string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString());
                    string[] files = Directory.GetFiles(e.Node.Tag.ToString(), "*.txt");


                    foreach (string fil in files)
                    {
                        DirectoryInfo fi = new DirectoryInfo(fil);
                        TreeNode node = new TreeNode(fi.Name, 0, 1);
                        try
                        {
                            node.Tag = fil;
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);

                        }

                    }

                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        TreeNode node = new TreeNode(di.Name, 0, 1);

                        try
                        {
                            //keep the directory's full path in the tag for use later
                            node.Tag = dir;

                            //if the directory has sub directories add the place holder
                            if (di.GetDirectories().Count() > 0)
                                node.Nodes.Add(null, "...", 0, 0);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //display a locked folder icon
                            node.ImageIndex = 12;
                            node.SelectedImageIndex = 12;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryLister",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }
                    }
                }
            }         
           
            //throw new NotImplementedException();
        }

        

        

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            fFilter1 = "*.txt";
            treeView1.SelectedNode.Collapse();
            treeView1.SelectedNode.Expand();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            fFilter1 = "*.jpg";
            treeView1.SelectedNode.Collapse();
            treeView1.SelectedNode.Expand();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            fFilter1 = "*.dll";
            treeView1.SelectedNode.Collapse();
            treeView1.SelectedNode.Expand();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            fFilter1 = "*.all";
            treeView1.SelectedNode.Collapse();
            treeView1.SelectedNode.Expand();
        }
    }
}



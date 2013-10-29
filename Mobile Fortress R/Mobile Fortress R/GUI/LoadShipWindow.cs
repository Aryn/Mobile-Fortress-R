using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;

namespace Mobile_Fortress_R.GUI
{
    class LoadShipWindow : ClosableWindow
    {
        public FileInfo Selection;

        ListControl list;
        ButtonControl OK;

        DirectoryInfo currentDirectory;
        string root;

        public LoadShipWindow(string root = "Ships")
        {
            this.root = root;
            currentDirectory = new DirectoryInfo(root);
            list = new ListControl();
            list.SetBounds(0.02f, 0, 0.1f, 0, 0.95f, 0, 0.75f, 0);
            list.SelectionMode = ListSelectionMode.Single;
            Children.Add(list);

            OK = new ButtonControl();
            OK.SetBounds(0.5f, -32, 1f, -28, 0, 64, 0, 24);
            OK.Text = "OK";
            OK.Pressed += new EventHandler(OK_Pressed);
            Children.Add(OK);
            
            UpdateOptions();
        }

        void OK_Pressed(object sender, EventArgs e)
        {
            string currentSelection = list.Items[list.SelectedItems[0]];
            if (currentSelection == "<--")
            {
                if (currentDirectory.Name == root) return;
                else
                {
                    currentDirectory = currentDirectory.Parent;
                    UpdateOptions();
                }
            }
            else if (currentSelection != null && currentSelection.Length > 0)
            {
                if (File.Exists(currentDirectory.ToString() + "/" + currentSelection))
                {
                    Selection = new FileInfo(currentDirectory.ToString() + "/" + currentSelection);
                    Close();
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(currentDirectory.ToString() + currentSelection);
                    currentDirectory = dir;
                    UpdateOptions();
                }
            }
        }
        void UpdateOptions()
        {
            list.Items.Clear();
            list.Items.Add("<--");
            foreach (DirectoryInfo dir in currentDirectory.EnumerateDirectories())
            {
                list.Items.Add("/" + dir.Name);
            }
            foreach (FileInfo file in currentDirectory.EnumerateFiles())
            {
                list.Items.Add(file.Name);
            }
            
        }

        public void Reset()
        {
            Selection = null;
            UpdateOptions();
        }
    }
}

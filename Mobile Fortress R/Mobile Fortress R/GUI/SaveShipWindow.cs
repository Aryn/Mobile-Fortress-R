using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface.Controls.Desktop;

namespace Mobile_Fortress_R.GUI
{
    class SaveShipWindow : ClosableWindow
    {
        public string FileName;
        public ChoiceBox Confirmation;

        ListControl list;
        InputControl fileNameInput;
        ButtonControl OK;

        DirectoryInfo currentDirectory;
        string root;

        public SaveShipWindow(string root = "Ships")
        {
            this.root = root;
            currentDirectory = new DirectoryInfo(root);
            list = new ListControl();
            list.SetBounds(0.02f, 0, 0.1f, 0, 0.95f, 0, 0.6f, 0);
            list.SelectionMode = ListSelectionMode.Single;
            Children.Add(list);

            fileNameInput = new InputControl();
            fileNameInput.SetBounds(0.5f, -64, 1f, -56, 0, 192, 0, 24);
            fileNameInput.Text = root + "/Ship.mf";
            Children.Add(fileNameInput);

            OK = new ButtonControl();
            OK.SetBounds(0.5f, -32, 1f, -28, 0, 64, 0, 24);
            OK.Text = "OK";
            OK.Pressed += new EventHandler(OK_Pressed);
            Children.Add(OK);

            Confirmation = new ChoiceBox("Overwrite Yo Momma.png?", new string[] { "OK", "Cancel" });
            
            UpdateOptions();
        }

        void OK_Pressed(object sender, EventArgs e)
        {
            FileName = RemoveInvalidCharacters(fileNameInput.Text);
            if (File.Exists(FileName))
            {
                Confirmation.Text = "Overwrite " + FileName + "?";
                MobileFortress.GUI.Screen.Desktop.Children.Add(Confirmation);
                Confirmation.BringToFront();
            }
            else
            {
                Close();
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

        string RemoveInvalidCharacters(string filename)
        {
            var newFileName = new StringBuilder();
            char[] invalid = Path.GetInvalidPathChars();
            for (int i = 0; i < filename.Length; i++)
            {
                char c = filename[i];
                if (!invalid.Contains<char>(c))
                {
                    newFileName.Append(c);
                }
            }
            return newFileName.ToString();
        }

        public void Reset()
        {
            FileName = null;
            Confirmation.Choice = null;
            UpdateOptions();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Screenshot_Program
{
    public partial class Form1 : Form
    {
        // Creating a custom key and a bool to check if a key is pressed.
        Keys sshotKey;
        bool isKeyPressed = false;

        // Create new global keyboard hook object.
        GlobalKeyboardHook gHook = new GlobalKeyboardHook();

        public Form1()
        {
            InitializeComponent();

            // Set key handling events for the global hook and add the keys.
            gHook.KeyDown += new KeyEventHandler(Form1_KeyDown);
            gHook.KeyUp += new KeyEventHandler(Form1_KeyUp);
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                gHook.HookedKeys.Add(key);
            }

            // Allows the form to handle key presses before controls, idk if it's required but w/e.
            this.KeyPreview = true;
            
            // Set the background image, images for labels, and images for pseudo-buttons (pictureboxes which act as buttons).
            this.BackgroundImage = Images.Background;
            pbLblLoc.Image = Images.SaveLocation;
            pbLblSshotKey.Image = Images.ScreenshotKey;
            pbSshotLocClear.Image = Images.Clear;
            pbOpenFolder.Image = Images.OpenFolder;
            pbSshotKeyClear.Image = Images.Clear;

            // Setting the active control to a label for the cleanse UI of dreams (this appears a lot in this program. :)
            ActiveControl = pbLblLoc;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Sets the values inside the textboxes to the saved default values when the form is opened and load the saved the location.
            txtSshotLoc.Text = Properties.Settings.Default.ScreenshotLocation;
            txtSshotKey.Text = Properties.Settings.Default.ScreenshotKey;
            this.Location = Properties.Settings.Default.Location;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Saves the values inside the textboxes and the location to the new default values when the form is closed.
            Properties.Settings.Default.ScreenshotLocation = txtSshotLoc.Text;
            Properties.Settings.Default.ScreenshotKey = txtSshotKey.Text;
            Properties.Settings.Default.Location = this.Location;
            Properties.Settings.Default.Save();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Changes the key value to the value in the textbox.
            Enum.TryParse(txtSshotKey.Text, out sshotKey);

            // Checks if the pressed key = the custom key and if the screenshot key textbox isn't highlighted
            // (because if you change the screenshot key to the same key then it will take a screenshot which shouldn't happen).
            if (e.KeyCode == sshotKey && txtSshotKey.Focused == false)
            {
                // If the key isn't already pressed do this.
                if (!isKeyPressed)
                {
                    // Call the Screenshot method.
                    Screenshot();

                    // Set isPressed to true.
                    isKeyPressed = true;

                    // Surpresses the key so that it doesn't interfere with other applications (i.e. if the key was F5 then it WON'T refresh a webpage).
                    e.Handled = true;
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Again, changes the key value to the value in the textbox.
            Enum.TryParse(txtSshotKey.Text, out sshotKey);

            // Checks if the un-pressed key = the custom key.
            if (e.KeyCode == sshotKey)
            {
                // If it's un-pressed, set the isPressed bool to false.
                isKeyPressed = false;
            }
        }

        private void txtSshotLoc_Click(object sender, EventArgs e)
        {
            // Opens a new FBD when the textbox is clicked.
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Sets the textbox to the selected directory.
                txtSshotLoc.Text = dialog.SelectedPath;
            }

            ActiveControl = pbLblLoc;
        }

        private void pbSshotLocClear_Click(object sender, EventArgs e)
        {
            // Resets the directory textbox.
            txtSshotLoc.Text = "C:";

            ActiveControl = pbLblLoc;
        }

        private void pbOpenFolder_Click(object sender, EventArgs e)
        {
            // Simply opens the folder where the screenshots are saved.
            try
            {
                string path = txtSshotLoc.Text;
                System.Diagnostics.Process.Start(path);
            }
            catch
            {

            }

            ActiveControl = pbLblLoc;
        }

        private void txtSshotKey_KeyDown(object sender, KeyEventArgs e)
        {
            // If the textbox is empty then do this.
            if (txtSshotKey.Text == "")
            {
                // Sets the textbox to the key which was pressed.
                txtSshotKey.Text += e.KeyCode.ToString();
            }
            // If the textbox isn't empty then do this.
            else
            {
                // If there was already a value within the textbox, then it clears it.
                // Then is sets the textbox to the key which was pressed.
                txtSshotKey.Clear();
                txtSshotKey.Text += e.KeyCode.ToString();
            }

            // Surpresses the key.
            e.Handled = true;

            ActiveControl = pbLblLoc;
        }

        private void pbSshotKeyClear_Click(object sender, EventArgs e)
        {
            // Clears the key textbox.
            txtSshotKey.Clear();

            ActiveControl = pbLblLoc;
        }

        #region MouseEnter & MouseLeave Events.

        // When the mouse enters the picturebox, it changes the image of the picturebox 
        // to a different one to show that it is selected and vice versa.
        private void pbSshotLocClear_MouseEnter(object sender, EventArgs e)
        {
            pbSshotLocClear.Image = Images.ClearSelected;
        }

        private void pbSshotLocClear_MouseLeave(object sender, EventArgs e)
        {
            pbSshotLocClear.Image = Images.Clear;
        }

        private void pbOpenFolder_MouseEnter(object sender, EventArgs e)
        {
            pbOpenFolder.Image = Images.OpenFolderSelected;
        }

        private void pbOpenFolder_MouseLeave(object sender, EventArgs e)
        {
            pbOpenFolder.Image = Images.OpenFolder;
        }

        private void pbSshotKeyClear_MouseEnter(object sender, EventArgs e)
        {
            pbSshotKeyClear.Image = Images.ClearSelected;
        }

        private void pbSshotKeyClear_MouseLeave(object sender, EventArgs e)
        {
            pbSshotKeyClear.Image = Images.Clear;
        }
        #endregion

        void Screenshot()
        {
            // Creates a new Rectangle with the bounds of the monitor.
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            // Creates a new Bitmap using the size of the Rectangle.
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                // Creates new graphics using the Bitmap.
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // Sets the InterpolationMode to high (idk if this actually changes anything lol).
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    // Copies the graphics from the screen using the size of the Rectangle.
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                // Creates a savePath string which takes the directory and adds on a unique part to the end based on DateTime.
                // (This means there aren't any overwrite issues.)
                string path = txtSshotLoc.Text + "\\Screenshot-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".bmp";
                // Saves the Bitmap to the savePath as a .bmp.
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }
    }
}

namespace FamilyTree
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Person_1_Addons = new GroupBox();
            addDad = new Button();
            button2 = new Button();
            addChild = new Button();
            addMom = new Button();
            deleteSelf = new Button();
            button1 = new Button();
            Person_1 = new GroupBox();
            Person_1_Addons.SuspendLayout();
            Person_1.SuspendLayout();
            SuspendLayout();
            // 
            // Person_1_Addons
            // 
            Person_1_Addons.Controls.Add(addDad);
            Person_1_Addons.Controls.Add(button2);
            Person_1_Addons.Controls.Add(addChild);
            Person_1_Addons.Controls.Add(addMom);
            Person_1_Addons.Controls.Add(deleteSelf);
            Person_1_Addons.Controls.Add(button1);
            Person_1_Addons.Location = new Point(161, 22);
            Person_1_Addons.Name = "Person_1_Addons";
            Person_1_Addons.Size = new Size(157, 140);
            Person_1_Addons.TabIndex = 9;
            Person_1_Addons.TabStop = false;
            Person_1_Addons.Text = "Options";
            Person_1_Addons.Visible = false;
            // 
            // addDad
            // 
            addDad.Location = new Point(90, 22);
            addDad.Name = "addDad";
            addDad.Size = new Size(56, 23);
            addDad.TabIndex = 3;
            addDad.Text = "+ Dad";
            addDad.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button2.ForeColor = SystemColors.ActiveCaptionText;
            button2.Location = new Point(13, 101);
            button2.Name = "button2";
            button2.Size = new Size(132, 23);
            button2.TabIndex = 8;
            button2.Text = "Upload picture";
            button2.UseVisualStyleBackColor = true;
            // 
            // addChild
            // 
            addChild.Location = new Point(14, 50);
            addChild.Name = "addChild";
            addChild.Size = new Size(55, 23);
            addChild.TabIndex = 4;
            addChild.Text = "+ Child";
            addChild.UseVisualStyleBackColor = true;
            // 
            // addMom
            // 
            addMom.Location = new Point(13, 22);
            addMom.Name = "addMom";
            addMom.Size = new Size(56, 23);
            addMom.TabIndex = 2;
            addMom.Text = "+ Mom";
            addMom.UseVisualStyleBackColor = true;
            // 
            // deleteSelf
            // 
            deleteSelf.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            deleteSelf.ForeColor = Color.Red;
            deleteSelf.Location = new Point(13, 76);
            deleteSelf.Name = "deleteSelf";
            deleteSelf.Size = new Size(133, 23);
            deleteSelf.TabIndex = 5;
            deleteSelf.Text = "DELETE SELF";
            deleteSelf.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(82, 50);
            button1.Name = "button1";
            button1.Size = new Size(64, 23);
            button1.TabIndex = 6;
            button1.Text = "+ Spouse";
            button1.UseVisualStyleBackColor = true;
            // 
            // Person_1
            // 
            Person_1.Controls.Add(Person_1_Addons);
            Person_1.Location = new Point(374, 12);
            Person_1.Name = "Person_1";
            Person_1.Size = new Size(324, 275);
            Person_1.TabIndex = 0;
            Person_1.TabStop = false;
            Person_1.Text = "Person";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1350, 729);
            Controls.Add(Person_1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            Person_1_Addons.ResumeLayout(false);
            Person_1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox Person_1_Addons;
        private Button addDad;
        private Button button2;
        private Button addChild;
        private Button addMom;
        private Button deleteSelf;
        private Button button1;
        private GroupBox Person_1;
    }
}
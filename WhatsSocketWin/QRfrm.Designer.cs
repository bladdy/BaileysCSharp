namespace WhatsSocketWin
    {
    partial class QRfrm
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            qrImg = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)qrImg).BeginInit();
            SuspendLayout();
            // 
            // qrImg
            // 
            qrImg.Dock = DockStyle.Fill;
            qrImg.Location = new Point(0, 0);
            qrImg.Name = "qrImg";
            qrImg.Size = new Size(462, 414);
            qrImg.SizeMode = PictureBoxSizeMode.StretchImage;
            qrImg.TabIndex = 0;
            qrImg.TabStop = false;
            // 
            // QRfrm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(462, 414);
            Controls.Add(qrImg);
            Name = "QRfrm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "QRfrm";
            Load += QRfrm_Load;
            ((System.ComponentModel.ISupportInitialize)qrImg).EndInit();
            ResumeLayout(false);
            }

        #endregion

        private PictureBox qrImg;
        }
    }
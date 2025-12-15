using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRenamerTool
{
    public partial class 文件批量重命名工具 : Form
    {
        public 文件批量重命名工具()
        {
            InitializeComponent();
        }

        // 浏览文件夹按钮点击事件
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "选择包含要重命名文件的文件夹";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFolderPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        // 开始重命名按钮点击事件
        private void btnStart_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtFolderPath.Text))
            {
                MessageBox.Show("请选择文件夹路径", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(txtFolderPath.Text))
            {
                MessageBox.Show("选择的文件夹不存在", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 执行重命名操作
            try
            {
                PerformRenaming();
                MessageBox.Show("完成", "操作完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作过程中发生错误: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 执行重命名操作
        private void PerformRenaming()
        {
            string folderPath = txtFolderPath.Text;
            string prefix = txtPrefix.Text.Trim();

            // 生成时间戳（精确到分钟）
            string timestamp = chkAddTimestamp.Checked ? DateTime.Now.ToString("yyyyMMdd_HHmm") : "";

            string[] files = Directory.GetFiles(folderPath);
            int renamedCount = 0;

            foreach (string filePath in files)
            {
                string fileName = Path.GetFileName(filePath);
                string extension = Path.GetExtension(filePath);
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

                // 构建新文件名 - 新的顺序：前缀_时间戳_原文件名
                List<string> nameParts = new List<string>();

                // 添加前缀（如果有）
                if (!string.IsNullOrWhiteSpace(prefix))
                    nameParts.Add(prefix);

                // 添加时间戳（如果有）- 现在放在前缀后面
                if (!string.IsNullOrEmpty(timestamp))
                    nameParts.Add(timestamp);

                // 添加原文件名
                nameParts.Add(nameWithoutExtension);

                // 构建新文件名
                string newFileName = string.Join("_", nameParts) + extension;

                // 如果新文件名与原文件名相同，跳过
                if (newFileName == fileName)
                    continue;

                string newFilePath = Path.Combine(folderPath, newFileName);

                try
                {
                    File.Move(filePath, newFilePath);
                    renamedCount++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"重命名失败: {fileName}\n错误: {ex.Message}", "错误");
                }
            }

            MessageBox.Show($"完成! 成功重命名 {renamedCount} 个文件", "操作完成");
        }

        private void 文件批量重命名工具_Load(object sender, EventArgs e)
        {

        }

        private void 关于应用ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }
    }
}

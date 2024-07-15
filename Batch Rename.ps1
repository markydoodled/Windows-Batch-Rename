param([string]$folderPath)

function InputBox {
    param(
        [string]$Message,
        [string]$WindowTitle = 'Input',
        [string]$DefaultText = ''
    )

    Add-Type -AssemblyName System.Windows.Forms
    $inputBox = New-Object System.Windows.Forms.Form 
    $inputBox.StartPosition = 'CenterScreen'
    $inputBox.Size = '300,150'
    $inputBox.TopMost = $true
    $inputBox.Text = $WindowTitle
    $textBox = New-Object System.Windows.Forms.TextBox 
    $textBox.Size = '200,20'
    $textBox.Location = '50,30'
    $textBox.Text = $DefaultText
    $okButton = New-Object System.Windows.Forms.Button
    $okButton.DialogResult = [System.Windows.Forms.DialogResult]::OK
    $okButton.Name = "okButton"
    $okButton.Size = '75,23'
    $okButton.Text = "OK"
    $okButton.Location = '50,60'
    $cancelButton = New-Object System.Windows.Forms.Button
    $cancelButton.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
    $cancelButton.Name = "cancelButton"
    $cancelButton.Size = '75,23'
    $cancelButton.Text = "Cancel"
    $cancelButton.Location = '150,60'
    $inputBox.AcceptButton = $okButton
    $inputBox.CancelButton = $cancelButton
    $inputBox.Controls.Add($textBox)
    $inputBox.Controls.Add($okButton)
    $inputBox.Controls.Add($cancelButton)
    $inputBox.Add_Shown({$inputBox.Activate()})
    $result = $inputBox.ShowDialog()

    if ($result -eq [System.Windows.Forms.DialogResult]::OK) {
        $textBox.Text
    }
}

if ($folderPath) {
    Get-ChildItem -Path $folderPath -File | ForEach-Object {
        $newName = $_.Directory.Name + " " + $_.BaseName + $_.Extension
        Rename-Item -Path $_.FullName -NewName $newName
    }
} else {
    Add-Type -AssemblyName System.Windows.Forms

    $form = New-Object System.Windows.Forms.Form
    $form.Text = 'Select Action'

    $button1 = New-Object System.Windows.Forms.Button
    $button1.Text = 'Choose Folder'
    $button1.Width = 100
    $button1.Height = 30
    $button1.Location = New-Object System.Drawing.Point(10, 10)
    $button1.Add_Click({
        $dialog = New-Object System.Windows.Forms.FolderBrowserDialog
        if ($dialog.ShowDialog() -eq 'OK') {
            $folderPath = $dialog.SelectedPath
            Get-ChildItem -Path $folderPath -File | ForEach-Object {
                $newName = $_.Directory.Name + " " + $_.BaseName + $_.Extension
                Rename-Item -Path $_.FullName -NewName $newName
            }
        }
        $form.Close()
    })
    $form.Controls.Add($button1)

    $button2 = New-Object System.Windows.Forms.Button
    $button2.Text = 'Specify Name'
    $button2.Width = 100
    $button2.Height = 30
    $button2.Location = New-Object System.Drawing.Point(120, 10)
    $button2.Add_Click({
        $name = InputBox 'Enter Name'
        if ($name) {
            # Use $name to rename files
            $dialog = New-Object System.Windows.Forms.FolderBrowserDialog
            if ($dialog.ShowDialog() -eq 'OK') {
                $folderPath = $dialog.SelectedPath
                Get-ChildItem -Path $folderPath -File | ForEach-Object {
                    $newName = $name + " " + $_.BaseName + $_.Extension
                    Rename-Item -Path $_.FullName -NewName $newName
                }
            }
        }
        $form.Close()
    })
    $form.Controls.Add($button2)

    $form.ShowDialog()
}
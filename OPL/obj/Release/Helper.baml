   M S B A M L   `   `   `  ���� >  :OPL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:OPLOPL      
OPL.Helper   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     #localclr-namespace:OPL     -     � Title$  
OPL Helper��$��505.03��$��505.03��$��920.71��$��920.71��.��� .+��� -    ��Name$ SourceDirectory_Textbox��$��23��$��147,11,131,0q� �� TextWrapping$
 Wrap=�$	��Top=��� $��Source Files Directory��$
��Left=�$��10,7,0,0q�$	��Top=�$	��132���� -    ��Name$ Scan_Button��$
��Scan��$��	0,12,10,0q�$	��Top=�$��22��$��Right=�$	��116��&   System.Windows.Controls.DataGrid  -     Name$ DataGrid��$��10,39,131,93q�   CanUserAddRows .     CanUserDeleteRows .     CanUserReorderColumns .     AutoGenerateColumns .  "	   VerticalScrollBarVisibility$
	 Auto=�$
   HorizontalScrollBarVisibility$

 Auto=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Game ID��   Width5  /System.Windows.Controls.DataGridLengthConverter$ 75    
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding ��  �� Path$ GameID� �� Mode$ OneWay=�  -   RegisteredName_Column��$ Registered Name��$ *  .  �� $ RegisteredName�$ OneWay=�  $ 	File Name��$ *  .  �� $
 Name�$ OneWay=�  $ New Name��$ *  �� $ ReName�$ TwoWay=�4  .System.Windows.Controls.DataGridCheckBoxColumn  $ Invalid�� �� $ Invalid�$ OneWay=��� -   $  ResetToOriginalName_Button��$��
Reset Name��$��	0,66,10,0q�$	��Top=�$��22��$��Right=�$	��116���� -   $  LimitToAllowedChars_Button��$��Trim Characters��$��
0,102,10,0q�$	��Top=�$��22��$��Right=�$	��116���� -   $! RemoveBracketContent_Button��$��Remove Brackets��$��
0,129,10,0q�$	��Top=�$��22��$��Right=�$	��116���� -	   $ Force32Chars_Button��$��Force 32 Chars��$��
0,156,10,0q�$	��Top=�$��22��$��Right=�$	��116���� -
   $ Apply_Button��$��Apply��$��	0,0,10,93q�$��Right=�$	��116��$��22��$��Bottom=��� -   $ Error_TextBox��$��
10,0,10,10q�$ WrapWithOverflow=� �� 
IsReadOnly . " �� VerticalScrollBarVisibility$
 Auto=�$ �� HorizontalScrollBarVisibility$
 Auto=�
���  ��$��78��$��Bottom=�
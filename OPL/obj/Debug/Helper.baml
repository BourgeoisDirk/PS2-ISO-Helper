   M S B A M L   `   `   `  ����>  :OPL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:OPLOPL  5   	       
OPL.Helper   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:OPL     5   	   -   5          � Title$  
OPL Helper��5   	   $��551.847��5	   	   $��551.847��6   $��972.508��5
   	   $��972.508��6   .��5      � .+�5   
   �� -   5         ��Name$ SourceDirectory_Textbox��5      $��23��63   $��147,11,131,0q�6?    �� TextWrapping$
 Wrap=�6U   $	��Top=�6i   �� 5   
   $��Source Files Directory��6   $
��Left=�61   $��10,7,0,0q�6L   $	��Top=�6^   $	��132��6v   �� 5   
   -   5         ��Name$ Scan_Button��5      $
��Scan��6&   $��	0,12,10,0q�65   $	��Top=�6H   $��22��6`   $��Right=�6�   $	��116��6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ DataGrid��5      $��10,39,131,93q�6%      CanUserAddRows .     CanUserDeleteRows .     CanUserReorderColumns .     AutoGenerateColumns .  "	   VerticalScrollBarVisibility$
	 Auto=�5   ~   $
   HorizontalScrollBarVisibility$

 Auto=�6�      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Game ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 75 66      
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 6S   ��  �� Path$ GameID� �� Mode$ OneWay=�  5      -   5        RegisteredName_Column��$ Registered Name��5   D   $ * 6]    6g   �� $ RegisteredName�$ TwoWay=�  5      $ 	File Name��6%   $ * 68    .  6T   �� $
 Name�$ OneWay=�  5      $ OPL Name��6%   $ * 67    6A   �� $ ReName�$ TwoWay=�  5      $ Length��6%   $ 50 65    .  6R   �� $ ReNameLength�$ OneWay=�4  .System.Windows.Controls.DataGridCheckBoxColumn  5      $ Invalid��6)   $ 50 6:    6E   �� $ Invalid�$ OneWay=�5      �� 5   
   -   5        $  ResetToOriginalName_Button��5      $��
Reset Name��65   $��	0,66,10,0q�6J   $	��Top=�6]   $��22��6u   $��Right=�6�   $	��116��6�   �� 5    
   -   5        $  LimitToAllowedChars_Button��5       $��Trim Characters��65   $��
0,102,10,0q�6O   $	��Top=�6c   $��22��6{   $��Right=�6�   $	��116��6�   �� 5!   
   -   5        $! RemoveBracketContent_Button��5!      $��Remove Brackets��66   $��
0,129,10,0q�6P   $	��Top=�6d   $��22��6|   $��Right=�6�   $	��116��6�   �� 5"   
   -	   5        $ Force32Chars_Button��5"      $��Force 32 Chars��6.   $��
0,156,10,0q�6G   $	��Top=�6[   $��22��6s   $��Right=�6�   $	��116��6�   �� 5$   
   -
   5        $ Apply_Button��5$      $��Apply��6'   $��	0,0,10,93q�67   $��Right=�6e   $	��116��6�   $��22��6�   $��Bottom=�6�   �� 5&   
   -   5        $ Error_TextBox��5&      $��
10,0,10,10q�6)   $ WrapWithOverflow=�6=    �� 
IsReadOnly . " �� VerticalScrollBarVisibility$
 Auto=�6o   $ �� HorizontalScrollBarVisibility$
 Auto=�6�   
���  ��$��78��6�   $��Bottom=�6�   5'      5(      
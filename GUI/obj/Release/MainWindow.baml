   M S B A M L   `   `   `  ���� K  GPS2 Image Reader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PS2_Image_ReaderPS2_Image_Reader  !    PS2_Image_Reader.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     0localclr-namespace:PS2_Image_Reader       � Title$  PS2 ISO Helper��$��605.826��$��605.826��$��802.849��$��802.849��.��� .+��� -    ��Name$ SourceDirectory_Textbox��$��23��$��228,11,13,0q� �� TextWrapping$
 Wrap=�$	��Top=��� -    ��Name$ 	Go_Button��$��Go��$��707,81,13,0q�$	��Top=��� $��Source Files Directory��$
��Left=�$��10,7,0,0q�$	��Top=�$	��213���� -   $ TargetBadISO_Textbox��$��23��$��228,46,13,0q�$
 Wrap=�$	��Top=�{�.  �� $��Failed Files Directory��$
��Left=�$��	10,42,0,0q�$	��Top=�$	��213���� -    ��Name$ OPLFriendly_CheckBox��$ ��Make filename OPL Friendly��$
��Left=�$��	13,82,0,0q�$	��Top=� d� 	IsChecked$
 TrueY��� -   $ LimitCharacters_CheckBox��$a��[Limit Characters to " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_()[]"��$
��Left=�$��
34,102,0,0q�$	��Top=�$
 TrueY��� -   $# RemoveBracketContent_CheckBox��$.��(Remove Brackets "(" and ")" with Content��$
��Left=�$��
34,117,0,0q�$	��Top=�$
 TrueY��� -   $$ ShortenTo32Characters_CheckBox��$.��(Force-Cut the Game Name to 32 Characters��$
��Left=�$��
34,132,0,0q�$	��Top=��� -   $ RunOplHelper_CheckBox��$��Run OPL Helper when Done��$
��Left=�$��524,154,0,0q�$	��Top=�$
 TrueY�$��RightToLeft=��� -	   $ 
OPL_Button��$��
OPL Helper��$��707,151,13,0q�$	��Top=��� -
    ��Name$ Status_TextBlock��$
��Left=�$��
10,183,0,0q�$
��Wrap=�$��Status��$	��Top=�$	��261��$��20��$��10(�� -    �Name$ 	StatusBar��$��20��$��276,183,13,0q�$	��Top=��� -   $ Output_Textbox��$��10,208,13,11q�$ NoWrap=� �� 
IsReadOnly . 
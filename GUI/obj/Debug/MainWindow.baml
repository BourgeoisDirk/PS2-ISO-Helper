   M S B A M L   `   `   `  ����K  GPS2 Image Reader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PS2_Image_ReaderPS2_Image_Reader  5   	   !    PS2_Image_Reader.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:PS2_Image_Reader     5   	     � Title$  PS2 ISO Helper��5   	   $��605.826��5	   	   $��605.826��6   $��802.849��5
   	   $��802.849��6   .��5      � .+�5   
   �� -   5         ��Name$ SourceDirectory_Textbox��5      $��23��63   $��228,11,13,0q�6?    �� TextWrapping$
 Wrap=�6T   $	��Top=�6h   6�   �� 5   
   -   5         ��Name$ 	Go_Button��5      $��Go��6$   $��707,81,13,0q�61   $	��Top=�6F   6^   �� 5   
   $��Source Files Directory��6   $
��Left=�61   $��10,7,0,0q�6L   $	��Top=�6^   $	��213��6v   �� 5   
   -   5        $ TargetBadISO_Textbox��5      $��23��60   $��228,46,13,0q�6<   $
 Wrap=�6Q   $	��Top=�6e   {�.  6}   �� 5   
   $��Failed Files Directory��6   $
��Left=�61   $��	10,42,0,0q�6L   $	��Top=�6_   $	��213��6w   �� 5   
   -   5         ��Name$ OPLFriendly_CheckBox��5      $ ��Make filename OPL Friendly��61   $
��Left=�6V   $��	13,82,0,0q�6q   $	��Top=�6�    d� 	IsChecked$
 TrueY�6�   6�   �� 5   
   -   5        $ LimitCharacters_CheckBox��5      $a��[Limit Characters to " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_()[]"��65   $
��Left=�6�   $��
34,102,0,0q�6�   $	��Top=�6�   $
 TrueY�6�   �� 5   
   -   5        $# RemoveBracketContent_CheckBox��5      $.��(Remove Brackets "(" and ")" with Content��6:   $
��Left=�6�   $��
34,117,0,0q�6�   $	��Top=�6�   $
 TrueY�6�   �� 5   
   -   5        $$ ShortenTo32Characters_CheckBox��5      $.��(Force-Cut the Game Name to 32 Characters��6;   $
��Left=�6n   $��
34,132,0,0q�6�   $	��Top=�6�   �� 5   
   -   5        $ RunOplHelper_CheckBox��5      $��Run OPL Helper when Done��62   $
��Left=�6U   $��524,154,0,0q�6p   $	��Top=�6�   $
 TrueY�6�   $��RightToLeft=�6�   �� 5   
   -	   5        $ 
OPL_Button��5      $��
OPL Helper��6%   $��707,151,13,0q�6:   $	��Top=�6P   6h   �� 5   
   -
   5         ��Name$ Status_TextBlock��5      $
��Left=�6.   $��
10,183,0,0q�6I   $
��Wrap=�6]   $��Status��6q   $	��Top=�6   $	��261��6�   $��20��6�   $��10(�6�   � 5   
   -   5         �Name$ 	StatusBar��5      $��20��6)   $��276,183,13,0q�65   $	��Top=�6K   �� 5   
   -   5        $ Output_Textbox��5      $��10,208,13,11q�6*   $ NoWrap=�6@    �� 
IsReadOnly . 6V   5      5      
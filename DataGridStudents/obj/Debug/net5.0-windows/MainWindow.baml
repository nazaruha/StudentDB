   M S B A M L   `   `   `  ����K  GDataGridStudents, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:DataGridStudentsDataGridStudents  5   	   !    DataGridStudents.MainWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:DataGridStudents     5   	     � Title$  
MainWindow��5   	   $	��441��6   $	��815��6)   .��5	      ]� .+�5
   
   &   System.Windows.Controls.DataGrid  -   5        $��
dgStudents��5
      
�������   RowBackground
 �����   AlternatingRowBackground
 ��ح�"   VerticalScrollBarVisibility$ Visible=�6}   $	��629��6�      AutoGenerateColumns$ False��6�      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Id��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6H   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Id+6H     5      $ FullName��6%   $	 200 6T    67   �� *
FullName+6T     5      $ Phone��6%   $	 130 6N    64   �� *Phone+6N     5      $ 	GroupName��6%    68   �� *	GroupName+  5      $ Date Create��6%   $	 110 6Y    6:   �� *
DateCreate+6Y   	   RowDetailsTemplate	 5      �� 5      .R�5      �� $��lvDataBinding��6   .@�5      �� ��6$   �� *SubjectsAndMarks+5      5      5      �� 5    
   $��Right=�6   .+�5!      �� -   5        $��	Reset_img��5!      $��/Reload-2-2-icon.png��6%   $��33��6C   $��38��6O   $��Right=�6Z   $��Stretch=�6v   $��0,0,2,0q�6�   �� 5"      -   5        $��
Create_btn��5"      $��0,13,0,0q�6'   $��24��69   $	��117��6E   $��16(�6Q   $��DemiBold"�6_   ���	#FF7EBF93.��6�   Create6�   �� 5#      -   5        $��
Update_btn��5#      $��0,13,0,0q�6'   $��24��69   $	��117��6E   $��16(�6Q   $��DemiBold"�6_   ���	#FFDEB368.��6�   Update6�   �� 5$      -   5        $��
Delete_btn��5$      $��0,13,0,0q�6'   $��24��69   $	��117��6E   $��16(�6Q   $��DemiBold"�6_   ���	#FFDE7D68.��6�   Delete6�   �� 5%      $��Stretch=�6   $��Stretch=�65   $��	0,211,0,0q�6S   .+�5&      �� -   5        $��TimeElapsed_txt��5&      
 �� 
IsReadOnly$

 True��61   $��9(�6C   $
��Bold"�6P    �� TextAlignment$ Center=�6b   $	��156��6y   $��Center=�6�   
���������6�   Z� 6�   � 5'      $��Stretch=�6   $��0,5,0,0q�68    � Orientation$ 
Horizontal=�6I   .+�5(      �� -   5        $��PrevPage_img��5(      $(��"/Actions-go-previous-view-icon.png��60   $��27��6\   $��29��6h   6s   �� 5)      -   5        $��PageOfPages_txt��5)      $

 True��65   $��1 of 10��6G   $��15(�6V   $
��Bold"�6d   $ Center=�6v   $	��113��6�   �� 5*      -	   5        $��NextPage_img��5*      $��27��60   $��29��6<   $$��/Actions-go-next-view-icon.png��6G   6o   5+      5,      5.      50      51      
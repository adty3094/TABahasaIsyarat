   M S B A M L   `   `   `  ����L  HTA Bahasa Isyarat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:TA_Bahasa_IsyaratTA_Bahasa_Isyarat  5   	   ^ ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers 5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 6{   "    TA_Bahasa_Isyarat.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TA_Bahasa_Isyarat     5   	   u
WpfViewers^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   	   9System&clr-namespace:System;assembly=mscorlib     6{   -   5          � Title$  
MainWindow��5	   	   $��774.5��6   $
��1063��6+   .��5
      � $��0,0,2,0q�6   .+�5   
   ;  5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  -   5          Name$ 	ColorView��5   '   $
��Left=�6:   $	��480��6U   $��	10,10,0,0q�6b   $	��Top=�6u   $	��640��6�   6  0Microsoft.Samples.Kinect.WpfViewers.KinectViewer   FlipHorizontally . 6�   >  8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer  5   
   -   5          Name$ SkeletonView��5   *   $
��Left=�6@   $	��480��6[   $��	10,10,0,0q�6h   $	��Top=�6{   $	��640��6�   $��RightToLeft=�6�   �� 5   
   -   5         ��Name$ StatusDetail��5      $	��asd��6&   $
��Left=�64   $��47��6O   $��109,686,0,0q�6[   $	��Top=�6p   $	��689��6�   $��24(�6�   �� 5   
   -   5        $ label1��5      $��Status:��6    $
��Left=�62   $��
10,686,0,0q�6M   $	��Top=�6a   $	��100��6y   $��24(�6�   $��47��6�   �� 5   
   -   5        $ label1_Copy��5      $��Create New Gesture��6%   $
��Left=�6B   $��
29,506,0,0q�6]   $	��Top=�6q   $	��152��6�   $��16(�6�   �� 5   
   -   5         ��Name$ fileName��5      $
��Left=�6$   $��28��6?   $��
35,578,0,0q�6K    �� TextWrapping$
 Wrap=�6_   $	��Top=�6s   $	��146��6�   $��16(�6�   �� 5   
   -   5         ��Name$ createButton��5      $��Create File��6'   $
��Left=�6=   $��
35,624,0,0q�6X   $	��Top=�6l   $��75��6�   6�   � 5   
   -	   5         �Name$ progressBar��5      $
��Left=�6+   $��20��6F   $��
35,666,0,0q�6R   $	��Top=�6f   $	��146��6~   �� 5   
   -
   5        $ label1_Copy1��5      $��Gesture Name:��6&   $
��Left=�6>   $��
29,542,0,0q�6Y   $	��Top=�6m   $	��115��6�   $��16(�6�   �� 5   
   -   5        	 ��Name$	 group1Gesture��5      $
��Left=�6)   $	��102��6D   $��396,542,0,0q�6Q   $	��Top=�6f   $	��182��6~   �� 5   
   -   5        $ label1_Copy2��5      $��Group 1 Gesture��6&   $
��Left=�6@   $��396,505,0,0q�6[   $	��Top=�6p   $	��152��6�   $��16(�6�   �� 5   
   -   5        $ label1_Copy3��5      $��Continued Testing��6&   $
��Left=�6B   $��859,118,0,0q�6]   $	��Top=�6r   $	��152��6�   $��16(�6�   �� 5   
   -   5        $ 
TestButton��5      $��Start Testing��6%   $
��Left=�6=   $��864,154,0,0q�6X   $	��Top=�6m   $��74��6�   6�   �� 5   
   -   5        $ label1_Copy4��5      $��Output Text��6&   $
��Left=�6<   $��859,211,0,0q�6W   $	��Top=�6l   $	��152��6�   $��16(�6�   �� 5   
   $
��Left=�6   $	��171��6,   $��661,247,0,0q�69   $	��Top=�6N   $	��171��6f   ._�5      �� -   5        
 ��Name$
 outputImage��5      $��UniformToFill=�6)   5      �� 5   
   -   5        $ label1_Copy5��5      $��Output Image��6&   $
��Left=�6=   $��655,211,0,0q�6X   $	��Top=�6m   $	��152��6�   $��16(�6�   �� 5   
   -   5        $ TrainButton��5      $��Train Dataset��6&   $
��Left=�6>   $��208,661,0,0q�6Y   $	��Top=�6n   $��76��6�   6�   �� 5   
   -   5         ��Name$ comboBox��5      $
��Left=�6%   $��208,550,0,0q�6@   $	��Top=�6U   $	��162��6m   .@�5       ��Back Propagation��6.   ��5!      '#Back Propagation Genetich Algorithm��6A   5"      �� 5#   
   -   5        $ label1_Copy6��5#      $��Choose Algorithm��6&   $
��Left=�6A   $��202,505,0,0q�6\   $	��Top=�6q   $	��140��6�   $��16(�6�   �� 5$   
   -   5        $ label1_Copy7��5$      $��BP Max iteration��6&   $
��Left=�6A   $��202,585,0,0q�6\   $	��Top=�6q   $	��140��6�   $��16(�6�   �� 5%   
   -   5        $ iteration_text��5%      $
��Left=�6*   $��23��6E   $��208,624,0,0q�6Q   $
 Wrap=�6f   $	��Top=�6z   $	��120��6�   �� 5&   
   -   5        $ label1_Copy8��5&      $��One Time Testing��6&   $
��Left=�6A   $��655,118,0,0q�6\   $	��Top=�6q   $	��152��6�   $��16(�6�   �� 5'   
   -   5        $ label1_Copy9��5'      $��Kinect Angle��6&   $
��Left=�6=   $��
655,10,0,0q�6X   $	��Top=�6l   $	��152��6�   $��16(�6�   �� 5(   
   -   5         ��Name$ slider��5(      $
��Left=�6!   $��
655,46,0,0q�6<   $	��Top=�6P   $��25��6h   $	��158��6t    � Maximum$ 25Q�6�    � Minimum$	 -25Q�6�    � SmallChange$ 1Q�6�    �� IsSnapToTickEnabled . 6�   �� 5)   
   -   5        $ label2��5)      $
��Left=�6N   $��
818,33,0,0q�6i   $	��Top=�6}   $��21.333(�6�   $��47��6�   ��6    �� *Value+ �� ElementName$ slider��6�   �� 5*   
   -   5        $ button��5*      $��	Set Angle��6!   $
��Left=�65   $��
655,76,0,0q�6P   $	��Top=�6d   $��75��6|   6�   �� 5+   
   -   5        $ label1_Copy10��5+      $��Group 2 Gesture��6'   $
��Left=�6A   $��598,506,0,0q�6\   $	��Top=�6q   $	��152��6�   $��16(�6�   �� 5,   
   -   5        $	 group2Gesture��5,      $
��Left=�6)   $	��102��6D   $��598,542,0,0q�6Q   $	��Top=�6f   $	��182��6~   �� 5-   
   -   5        $ OneTestButton��5-      $��Start Testing��6(   $
��Left=�6@   $��661,154,0,0q�6[   $	��Top=�6p   $��75��6�   6�   �� 5.   
   
���   �$��1q�6%   $
��Left=�69   $��72��6T   $��655,118,0,0q�6`   $	��Top=�6u   $	��152��6�   �� 5/   
   
���   �$��1q�6%   $
��Left=�69   $��72��6T   $��859,118,0,0q�6`   $	��Top=�6u   $	��152��6�   �� 50   
   
���   �$��1q�6%   $
��Left=�69   $	��171��6T   $��661,247,0,0q�6a   $	��Top=�6v   $	��171��6�   �� 51   
   
���   �$��1q�6%   $
��Left=�69   $��72��6T   $��864,247,0,0q�6`   $	��Top=�6u   $	��179��6�   �� 52   
   -   5        $ 
outputText��52      $�� ��6$   $
��Left=�6/   $��864,256,0,0q�6J   $	��Top=�6_   $��48��6w   $	��179��6�   $��26.667(�6�   �� 53   
   -    5        $ label3��53      $	��Log��6    $
��Left=�6.   $��794,452,0,0q�6I   $	��Top=�6^   �� 54   
   -!   5        $ label4��54      $��ERWR.Y��6    $
��Left=�61   $��794,483,0,0q�6L   $	��Top=�6a   �� 55   
   -"   5        $ 	Log_ERWRY��55      $�� ��6#   $
��Left=�6.   $��875,483,0,0q�6I   $	��Top=�6^   $	��168��6v   �� 56   
   -#   5        $ label5��56      $��Neural Network Output��6    $
��Left=�6@   $��794,546,0,0q�6[   $	��Top=�6p    J� RenderTransformOrigin$ 0.489,1.154&�6�   �� 57   
   -$   5        $	 
outputList��57      $
��Left=�6&   $	��144��6A   $��803,578,0,0q�6N   $	��Top=�6c   $	��135��6{   �� 58   
   
���   �$��1q�6%   $
��Left=�69   $	��275��6T   $��789,452,0,0q�6a   $	��Top=�6v   $	��254��6�   �� 59   
   -%   5        $ label5_Copy��59      $��Actual Class��6%   $
��Left=�6<   $��794,515,0,0q�6W   $	��Top=�6l   $ 0.489,1.154&�6�   �� 5:   
   -&   5        $ Log_ActualClass��5:      $�� ��6)   $
��Left=�64   $��875,515,0,0q�6O   $	��Top=�6d   $	��168��6|   5<      5=      
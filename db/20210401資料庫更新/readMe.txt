1. 調整：Midterm.dbo.Routes 設計
   原因：新北市RouteName內容長度超過原先設定的範圍
   結果：資料行RouteName的資料類型nvachar(10)→nvachar(50)

2. 調整：Midterm.dbo.Routes 新增資料
   原因：增加新北市路線
   結果：資料筆數497→1138(+641)

3. 調整：Midterm.dbo.GarbageTruckSpots 新增資料
   原因：增加新北市收集點
   結果：資料筆數4015→30392(+26377)
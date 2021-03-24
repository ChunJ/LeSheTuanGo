/*
 * Javascript 共用 function 說明：
 * 
 * getDistrict("distTagId") 和 getProduct(prodTagId) 使用方法一樣，
 * 這邊以 getDistrict 來做說明。
 * getDistrict("distTagId")放在城市選單的onchange事件裡，
 * 裡面的參數是鄉鎮市區選單的id(前面要加"#") 。
 *
 * 範例：
 * 
 * 城市
 * <select onchange="getDistrict('#DistrictId')"></select>
 * 鄉鎮市區
 * <select id="DistrictId"></select>
*/
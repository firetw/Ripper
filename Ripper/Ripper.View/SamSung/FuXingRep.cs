﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamSung
{
    public class FuXingRep
    {
        public static List<string> Map = new List<string>();

        static FuXingRep()
        {
            Encoding gbk = Encoding.UTF8;
            /*
            string fuxing = @"A  安陵 安平 安期 安阳
B  白马 百里 柏侯 鲍俎 北宫 北郭 北门 北山 北唐 奔水 逼阳 宾牟 薄奚 薄野
C  曹牟 曹丘 常涛 长鱼 车非成功 成阳 乘马 叱卢 丑门 樗里 穿封 淳子
D  答禄 达勃 达步 达奚 淡台 邓陵 第五 地连 地伦 东方 东里 东南 东宫 东门 东乡东丹 东郭 东陵 东关 东闾 东阳 东野 东莱 豆卢 斗于 都尉 独孤 端木 段干 多子
E  尔朱
F  方雷 丰将 封人 封父 夫蒙 夫馀 浮丘傅余
G  干已 高车 高陵 高堂 高阳 高辛 皋落 哥舒 盖楼 庚桑 梗阳 宫孙 公羊 公良 公孙 公罔 公西 公冶 公敛 公梁 公输 公上公山 公户 公玉 公仪 公仲 公坚 公伯 公祖 公乘 公晰 公族 姑布 古口 古龙 古孙 谷梁 谷浑 瓜田 关龙 鲑阳 归海
H  函治 韩馀罕井 浩生 浩星 纥骨 纥奚 纥于 贺拨 贺兰 贺楼 赫连 黑齿 黑肱 侯冈 呼延 壶丘 呼衍 斛律 胡非 胡母 胡毋 皇甫 皇父
J  兀官吉白 即墨 季瓜 季连 季孙 茄众 蒋丘 金齿 晋楚 京城 泾阳 九百 九方 睢鸠 沮渠 巨母
K  勘阻 渴侯 渴单 可汗 空桐 空相 昆吾
L  老阳 乐羊 荔菲 栎阳 梁丘 梁由 梁馀 梁垣 陵阳 伶舟 冷沦 令狐 刘王 柳下 龙丘 卢妃 卢蒲 鲁步 陆费 角里 闾丘
M  马矢 麦丘茅夷 弥牟 密革 密茅 墨夷 墨台 万俊 昌顿 慕容 木门 木易
N  南宫 南郭 南门 南荣
O  欧侯 欧阳
P  逄门 盆成 彭祖 平陵 平宁 破丑仆固 濮阳
Q  漆雕 奇介 綦母 綦毋 綦连 祁连 乞伏 绮里 千代 千乘 勤宿 青阳 丘丽 丘陵 屈侯 屈突 屈男 屈卢 屈同 屈门 屈引
R  壤四 扰龙 容成 汝嫣
S  萨孤 三饭 三闾 三州 桑丘 商瞿 上官 尚方 少师 少施 少室 少叔 少正 社南 社北 申屠 申徒 沈犹 胜屠石作 石牛 侍其 士季 士弱 士孙 士贞 叔孙 叔先 叔促 水丘 司城 司空 司寇 司鸿 司马 司徒 司士 似和 素和 夙沙 孙阳 索阳 索卢
T  沓卢 太史 太叔 太阳 澹台 唐山 堂溪 陶丘 同蹄 统奚 秃发 涂钦 吐火 吐贺 吐万 吐罗 吐门 吐难 吐缶 吐浑 吐奚 吐和 屯浑脱脱 拓拨
W  完颜 王孙 王官 王人 微生 尾勺 温孤 温稽 闻人 屋户 巫马 吾丘 无庸 无钩 五鹿
X  息夫 西陵 西乞 西钥 西乡 西门西周 西郭 西方 西野 西宫 戏阳 瑕吕 霞露 夏侯 鲜虞 鲜于 鲜阳 咸丘 相里 解枇 谢丘 新垣 辛垣 信都 信平 修鱼 徐吾 宣于 轩辕轩丘 阏氏
Y  延陵 罔法 铅陵 羊角 耶律 叶阳 伊祁 伊耆 猗卢 义渠 邑由 因孙 银齿 尹文 雍门 游水 由吾 右师 宥连 於陵 虞丘盂丘 宇文 尉迟 乐羊 乐正 运奄 运期
Z  宰父 辗迟 湛卢 章仇 仉督 长孙 长儿 真鄂 正令 执头 中央 中长 中行 中野 中英 中梁中垒 钟离 钟吾 终黎 终葵 仲孙 仲长 周阳 周氏 周生 朱阳 诸葛 主父 颛孙 颛顼 訾辱 淄丘 子言 子人 子服 子家 子桑 子叔 子车子阳 宗伯 宗正 宗政 尊卢 昨和 左人 左丘 左师 左行 刘文 额尔 达力 蔡斯 浩赏 斛斯 夹谷 揭阳 ";
           */
            string fuxing = @"欧阳 太史 端木 上官 司马 东方 独孤 南宫 万俟 闻人 夏侯 诸葛 尉迟 公羊 赫连 澹台 皇甫 宗政 濮阳 公冶 太叔 申屠 公孙 慕容 仲孙 钟离 长孙 宇文 司徒 鲜于 司空 闾丘 子车 亓官 司寇 巫马 公西 颛孙 壤驷 公良 漆雕 乐正 宰父 谷梁 拓跋 夹谷 轩辕 令狐 段干 百里 呼延 东郭 南门 羊舌 微生 公户 公玉 公仪 梁丘 公仲 公上 公门 公山 公坚 左丘 公伯 西门 公祖 第五 公乘 贯丘 公皙 南荣 东里 东宫 仲长 子书 子桑 即墨 达奚 褚师";

            string line = string.Empty;

            string[] lastNames = fuxing.Split(' ');
            for (int i = 1; i < lastNames.Length; i++)
            {
                string value = lastNames[i].Trim();

                Map.Add(value);
            }
        }



    }
}
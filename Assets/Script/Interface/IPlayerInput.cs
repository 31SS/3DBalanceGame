//キーボード等の操作機器を容易に切り替えられるように用意したインターフェース
//現在はキーボードのみ対応
public interface IPlayerInput
{
    void Inputting();

    float X
    {
        get; set;
    }

    bool Jump
    {
        get; set;
    }
}
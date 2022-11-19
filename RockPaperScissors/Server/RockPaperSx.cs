using Common;

namespace Server;

public class RockPaperSx : Singleton<RockPaperSx>
{
    public Int32 RockPaper(Int32 player1, Int32 player2)
    {
        switch (player1)
        {
            case (Int32)Enums.ROCK:
                if (player2 == (Int32)Enums.ROCK)
                {
                    Console.WriteLine("비겼습니다.");
                    return (Int32)Enums.DREW;
                }
                else if (player2 == (Int32)Enums.PAPER)
                {
                    Console.WriteLine("졌습니다");
                    return (Int32)Enums.LOOSE;
                }
                else
                {
                    Console.WriteLine("이겼습니다");
                    return (Int32)Enums.WIN;
                }
                break;
            case (Int32)Enums.PAPER:
                if (player2 == (Int32)Enums.PAPER)
                {
                    Console.WriteLine("비겼습니다.");
                    return (Int32)Enums.DREW;
                }
                else if (player2 == (Int32)Enums.SCISSORS)
                {
                    Console.WriteLine("졌습니다");
                    return (Int32)Enums.LOOSE;
                }
                else
                {
                    Console.WriteLine("이겼습니다");
                    return (Int32)Enums.WIN;
                }
                break;
            case (Int32)Enums.SCISSORS:
                if (player2 == (Int32)Enums.SCISSORS)
                {
                    Console.WriteLine("비겼습니다.");
                    return (Int32)Enums.DREW;
                }

                else if (player2 == (Int32)Enums.ROCK)
                {
                    Console.WriteLine("졌습니다");
                    return (Int32)Enums.LOOSE;
                }
                else
                {
                    Console.WriteLine("이겼습니다");
                    return (Int32)Enums.WIN;
                }
                break;
        }
        return -1;
    }
}
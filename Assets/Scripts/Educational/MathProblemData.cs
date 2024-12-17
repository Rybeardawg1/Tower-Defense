using System.Collections.Generic;

public static class MathProblemData
{
    // Easy Problems
    public static readonly List<(string Problem, string Solution)> EasyProblems = new List<(string, string)>
    {
        ("What is 25 + 17?", "42"),
        ("What is 48 - 23?", "25"),
        ("If 5 × 3 = ?", "15"),
        ("What is 45 ÷ 9?", "5"),
        ("Simplify: 2 + 3 × 4", "14"),
        ("Find the value of x if x + 8 = 15.", "7"),
        ("Convert 3/4 into a decimal.", "0.75"),
        ("What is the perimeter of a square with side length 6?", "24"),
        ("If a pencil costs $2, how much do 5 pencils cost?", "10"),
        ("What is the area of a rectangle with length 8 and width 3?", "24"),
        ("What is 10% of 50?", "5"),
        ("Solve: 100 - (25 + 15)", "60"),
        ("If a car travels 60 km in 2 hours, what is its speed?", "30"),
        ("Write 3/5 as a percentage.", "60%"),
        ("What is the value of 6²?", "36"),
        ("If a triangle has angles of 60° and 50°, what is the third angle?", "70"),
        ("If 1 apple costs $3, how much do 7 apples cost?", "21"),
        ("How many minutes are there in 2 hours?", "120"),
        ("Solve: 4³", "64"),
        ("What is the largest prime number less than 10?", "7")
    };

    // Medium Problems
    public static readonly List<(string Problem, string Solution)> MediumProblems = new List<(string, string)>
    {
        ("Solve: 4x = 20. Find x.", "5"),
        ("Simplify: (3 × 5) + (12 ÷ 3)", "19"),
        ("What is 15% of 200?", "30"),
        ("What is the area of a circle with radius 7? Round off answer. (Use π = 3.14)", "154"),
        ("If a = 5 and b = 2, find the value of a² - b².", "21"),
        ("What is the sum of the first 5 prime numbers?", "28"),
        ("Solve: 3x + 5 = 14.", "3"),
        ("What is the volume of a cube with a side length of 4?", "64"),
        ("If a train travels 240 km in 4 hours, what is its speed?", "60"),
        ("What is 2/3 of 18?", "12"),
        ("Simplify: 5² - 3 × 2 + 1", "20"),
        ("If 4 books cost $48, how much does 1 book cost?", "12"),
        ("Solve: x/3 = 12. Find x.", "36"),
        ("What is the circumference of a circle with diameter 10? (Use π = 3.14)", "31.4"),
        ("Find the LCM of 4 and 6.", "12"),
        ("What is the average of 12, 18, and 24?", "18"),
        ("A shopkeeper sells a pen for $5 with a 20% discount. What is the discounted price?", "4"),
        ("If x = 3, solve: x² + 4x + 5.", "26"),
        ("What is the square root of 81?", "9"),
        ("If a rectangle has a length of 10 and a diagonal of 13, find the width.", "6")
    };

    // Hard Problems
    public static readonly List<(string Problem, string Solution)> HardProblems = new List<(string, string)>
    {
        ("Solve: 5x + 3 = 23.", "4"),
        ("Simplify: (3² + 4²) ÷ 2", "12.5"),
        ("If x + y = 12 and x - y = 4, find x.", "8"),
        ("What is the value of 7³?", "343"),
        ("Find the HCF of 54 and 24.", "6"),
        ("If a triangle has sides 3, 4, and 5, what is its area?", "6"),
        ("Solve: x² - 4x - 21 = 0.", "x = 7 or x = -3"),
        ("Find the cube root of 512.", "8"),
        ("If a = 2 and b = 3, find the value of a^b + b^a.", "17"),
        ("A cone has a radius of 4 and height of 9. Find its volume. Round off answer. (Use π = 3.14)", "151"),
        ("Solve: 2x + 5y = 20, x = 2. Find y.", "3"),
        ("What is the probability of rolling a 4 on a fair six-sided die?", "1/6"),
        ("If the perimeter of a rectangle is 28 and its length is 9, find its width.", "5"),
        ("Find the sum of angles in a pentagon.", "540"),
        ("Solve: sqrt(49) + 3³ - 4²", "18"),
        ("If a circle has an area of 78.5 (π = 3.14), find its radius.", "5"),
        ("If x + 3y = 15 and x - y = 5, solve for y.", "2.5"),
        ("Solve for x: 2^(x+1) = 16.", "3"),
        ("Find the surface area of a cube with side 5.", "150"),
        ("If a number is increased by 50% and then decreased by 30%, what is the net change?", "5")
    };
}

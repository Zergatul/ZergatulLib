function hex($params)
{
    $i = $params[0]
    $digits = $params[1]
    if (-not $digits)
    {
        $digits = 1
    }
    return [System.Convert]::ToString($i, 16).PadLeft($digits, '0')
}

$FFT128_8_16_Twiddle =
@(
1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
1, 60, 2, 120, 4, -17, 8, -34, 16, -68, 32, 121, 64, -15, 128, -30, 
1, 46, 60, -67, 2, 92, 120, 123, 4, -73, -17, -11, 8, 111, -34, -22, 
1, -67, 120, -73, 8, -22, -68, -70, 64, 81, -30, -46, -2, -123, 17, -111, 
1, -118, 46, -31, 60, 116, -67, -61, 2, 21, 92, -62, 120, -25, 123, -122, 
1, 116, 92, -122, -17, 84, -22, 18, 32, 114, 117, -49, -30, 118, 67, 62, 
1, -31, -67, 21, 120, -122, -73, -50, 8, 9, -22, -89, -68, 52, -70, 114, 
1, -61, 123, -50, -34, 18, -70, -99, 128, -98, 67, 25, 17, -9, 35, -79
)

$FFT256_2_128_Twiddle =
@(
   1,   41, -118,   45,   46,   87,  -31,   14, 
  60, -110,  116, -127,  -67,   80,  -61,   69, 
   2,   82,   21,   90,   92,  -83,  -62,   28, 
 120,   37,  -25,    3,  123,  -97, -122, -119, 
   4,  -93,   42,  -77,  -73,   91, -124,   56, 
 -17,   74,  -50,    6,  -11,   63,   13,   19, 
   8,   71,   84,  103,  111,  -75,    9,  112, 
 -34, -109, -100,   12,  -22,  126,   26,   38, 
  16, -115,  -89,  -51,  -35,  107,   18,  -33, 
 -68,   39,   57,   24,  -44,   -5,   52,   76, 
  32,   27,   79, -102,  -70,  -43,   36,  -66, 
 121,   78,  114,   48,  -88,  -10,  104, -105, 
  64,   54,  -99,   53,  117,  -86,   72,  125, 
 -15, -101,  -29,   96,   81,  -20,  -49,   47, 
 128,  108,   59,  106,  -23,   85, -113,   -7, 
 -30,   55,  -58,  -65,  -95,  -40,  -98,   94
)

function EXTRA_REDUCE_S($x)
{
    "$x <= 128 ? $x : $x - 257"
}

function FFT_8($params)
{
    $index = $params[0]
    $stripe = $params[1]

    function X($i)
    {
        "y[$index + $stripe * $i]"
    }

    function DO_REDUCE($i)
    {
        Write-Output "$(X($i)) = Reduce($(X($i)));"
    }

    function DO_REDUCE_FULL_S($i)
    {
        DO_REDUCE($i)
        Write-Output "$(X($i)) = $(EXTRA_REDUCE_S(X($i)));"
    }

    function BUTTERFLY($params)
    {
        $i = $params[0]
        $j = $params[1]
        $n = $params[2]

        Write-Output "u = $(X($i));"
        Write-Output "v = $(X($j));"
        Write-Output "$(X($i)) = u + v;"
        Write-Output "$(X($j)) = (u - v) << $(2 * $n);"
    }

    Write-Output "int u, v;"

    BUTTERFLY(0, 4, 0);
    BUTTERFLY(1, 5, 1);
    BUTTERFLY(2, 6, 2);
    BUTTERFLY(3, 7, 3);

    DO_REDUCE(6);
    DO_REDUCE(7);

    BUTTERFLY(0, 2, 0);
    BUTTERFLY(4, 6, 0);
    BUTTERFLY(1, 3, 2);
    BUTTERFLY(5, 7, 2);

    DO_REDUCE(7);
    
    BUTTERFLY(0, 1, 0);
    BUTTERFLY(2, 3, 0);
    BUTTERFLY(4, 5, 0);
    BUTTERFLY(6, 7, 0);

    DO_REDUCE_FULL_S(0);
    DO_REDUCE_FULL_S(1);
    DO_REDUCE_FULL_S(2);
    DO_REDUCE_FULL_S(3);
    DO_REDUCE_FULL_S(4);
    DO_REDUCE_FULL_S(5);
    DO_REDUCE_FULL_S(6);
    DO_REDUCE_FULL_S(7);
}

function FFT_16($params)
{
    $index = $params[0]
    $stripe = $params[1]

    
    function X($i)
    {
        "y[$index + $stripe * $i]"
    }

    function DO_REDUCE($i)
    {
        Write-Output "$(X($i)) = Reduce($(X($i)));"
    }

    function DO_REDUCE_FULL_S($i)
    {
        DO_REDUCE($i)
        Write-Output "$(X($i)) = $(EXTRA_REDUCE_S(X($i)));"
    }

    function BUTTERFLY($params)
    {
        $i = $params[0]
        $j = $params[1]
        $n = $params[2]

        Write-Output "u = $(X($i));"
        Write-Output "v = $(X($j));"
        Write-Output "$(X($i)) = u + v;"
        Write-Output "$(X($j)) = (u - v) << $n;"
    }

    Write-Output "int u, v;"

    BUTTERFLY(0,  8, 0);
    BUTTERFLY(1,  9, 1);
    BUTTERFLY(2, 10, 2);
    BUTTERFLY(3, 11, 3);
    BUTTERFLY(4, 12, 4);
    BUTTERFLY(5, 13, 5);
    BUTTERFLY(6, 14, 6);
    BUTTERFLY(7, 15, 7);

    DO_REDUCE(11);
    DO_REDUCE(12);
    DO_REDUCE(13);
    DO_REDUCE(14);
    DO_REDUCE(15);

    BUTTERFLY( 0,  4, 0);
    BUTTERFLY( 8, 12, 0);
    BUTTERFLY( 1,  5, 2);
    BUTTERFLY( 9, 13, 2);
    BUTTERFLY( 2,  6, 4);
    BUTTERFLY(10, 14, 4);
    BUTTERFLY( 3,  7, 6);
    BUTTERFLY(11, 15, 6);

    DO_REDUCE(5);
    DO_REDUCE(7);
    DO_REDUCE(13);
    DO_REDUCE(15);
    
    BUTTERFLY( 0,  2, 0);
    BUTTERFLY( 4,  6, 0);
    BUTTERFLY( 8, 10, 0);
    BUTTERFLY(12, 14, 0);
    BUTTERFLY( 1,  3, 4);
    BUTTERFLY( 5,  7, 4);
    BUTTERFLY( 9, 11, 4);
    BUTTERFLY(13, 15, 4);

    BUTTERFLY( 0,  1, 0);
    BUTTERFLY( 2,  3, 0);
    BUTTERFLY( 4,  5, 0);
    BUTTERFLY( 6,  7, 0);
    BUTTERFLY( 8,  9, 0);
    BUTTERFLY(10, 11, 0);
    BUTTERFLY(12, 13, 0);
    BUTTERFLY(14, 15, 0);

    DO_REDUCE_FULL_S( 0);
    DO_REDUCE_FULL_S( 1);
    DO_REDUCE_FULL_S( 2);
    DO_REDUCE_FULL_S( 3);
    DO_REDUCE_FULL_S( 4);
    DO_REDUCE_FULL_S( 5);
    DO_REDUCE_FULL_S( 6);
    DO_REDUCE_FULL_S( 7);
    DO_REDUCE_FULL_S( 8);
    DO_REDUCE_FULL_S( 9);
    DO_REDUCE_FULL_S(10);
    DO_REDUCE_FULL_S(11);
    DO_REDUCE_FULL_S(12);
    DO_REDUCE_FULL_S(13);
    DO_REDUCE_FULL_S(14);
    DO_REDUCE_FULL_S(15);
}

function FFT_128_full($index)
{
    Write-Output '#region FFT_128_full'
    Write-Output ''

    for ($i = 0; $i -lt 16; $i++)
    {
        FFT_8(($index + $i), 16)
    }

    Write-Output ''

    for ($i = 0; $i -lt 128; $i++)
    {
        Write-Output "x = $($FFT128_8_16_Twiddle[$i]) * y$(hex($i, 2));"
        Write-Output "y$(hex($i, 2)) = (x & 0xFF) - (x >> 8);"
    }

    for ($i = 0; $i -lt 8; $i++)
    {
        FFT_16(($index + $i * 16), 1)
    }

    Write-Output ''
    Write-Output '#endregion'
}

function FFT_256_halfzero
{
    Write-Output '#region FFT_256_halfzero'
    Write-Output ''
    Write-Output 'int tmp = y7f;'
    Write-Output 'int x;'
    Write-Output ''
    
    for ($i = 0; $i -lt 127; $i++)
    {
        Write-Output "x = $($FFT256_2_128_Twiddle[$i]) * y$(hex($i, 2));"
        Write-Output "int y$(hex((128 + $i), 2)) = (x & 0xFF) - (x >> 8);"
    }
    Write-Output ''

    Write-Output 'if (final)'
    Write-Output '{'
    Write-Output 'x = y7d + 1;'
    Write-Output 'y7d = (x & 0xFF) - (x >> 8);';
    Write-Output 'x = -40 * (x - 2);'
    Write-Output 'yfd = (x & 0xFF) - (x >> 8);';
    Write-Output '}'
    Write-Output ''
    Write-Output 'x = tmp + 1;'
    Write-Output 'y7f = (x & 0xFF) - (x >> 8);';
    Write-Output 'x = 94 * (tmp - 1);'
    Write-Output 'int yff = (x & 0xFF) - (x >> 8);';
    Write-Output ''
    FFT_128_full(0)
    Write-Output ''
    FFT_128_full(128)
    Write-Output ''
    Write-Output '#endregion'
}

function STEP4($params)
{
    function Convert($part)
    {
        switch ($part)
        {
            'A' { 0 }
            'B' { 4 }
            'C' { 8 }
            'D' { 12 }
        }
    }

    $state = $params[0]
    $w = $params[1]
    $i = $params[2]
    $r = $params[3]
    $s = $params[4]
    $A = Convert($params[5])
    $B = Convert($params[6])
    $C = Convert($params[7])
    $D = Convert($params[8])
    $F = $params[9]

    Write-Output "for (int j = 0; j < 4; j++)"
    Write-Output "R[j] = RotateLeft($state[$A + j], $r);"

    Write-Output "for (int j = 0; j < 4; j++)"
    Write-Output "{"
    Write-Output "$state[$D + j] = $state[$D + j] + $w[j] + $F($state[$A + j], $state[$B + j], $state[$C + j]);"
    Write-Output "$state[$D + j] = RotateLeft($state[$D + j], $s) + R[j ^ ($i % 3 + 1)];"
    Write-Output "$state[$A + j] = R[j];"
    Write-Output "}"

    Write-Output ""
}

function STEP8($params)
{
    function Convert($part)
    {
        switch ($part)
        {
            'A' { 0 }
            'B' { 8 }
            'C' { 16 }
            'D' { 24 }
        }
    }

    $state = $params[0]
    $w = $params[1]
    $i = $params[2]
    $r = $params[3]
    $s = $params[4]
    $A = Convert($params[5])
    $B = Convert($params[6])
    $C = Convert($params[7])
    $D = Convert($params[8])
    $F = $params[9]

    Write-Output "for (int j = 0; j < 8; j++)"
    Write-Output "R[j] = RotateLeft($state[$A + j], $r);"

    Write-Output "for (int j = 0; j < 8; j++)"
    Write-Output "{"
    Write-Output "$state[$D + j] = $state[$D + j] + $w[j] + $F($state[$A + j], $state[$B + j], $state[$C + j]);"
    Write-Output "$state[$D + j] = RotateLeft($state[$D + j], $s) + R[j ^ P8Xor[$i % 7]];"
    Write-Output "$state[$A + j] = R[j];"
    Write-Output "}"

    Write-Output ""

}

function ProcessBlock512
{
    for ($i = 0; $i -lt 128; $i++)
    {
        Write-Output "int y$(hex($i, 2)) = buffer[0x$(hex($i, 2))];"
    }

    Write-Output ''

    FFT_256_halfzero

    Write-Output ''
}

Clear-Host
FFT_8('index', 'stripe')
#FFT_16('index', 'stripe')
                             
#STEP8('state', 'w[0]', '(8*i+0)', 'r', 's', 'A', 'B', 'C', 'D', 'IF');
#STEP8('state', 'w[1]', '(8*i+1)', 's', 't', 'D', 'A', 'B', 'C', 'IF');
#STEP8('state', 'w[2]', '(8*i+2)', 't', 'u', 'C', 'D', 'A', 'B', 'IF');
#STEP8('state', 'w[3]', '(8*i+3)', 'u', 'r', 'B', 'C', 'D', 'A', 'IF');
#STEP8('state', 'w[4]', '(8*i+4)', 'r', 's', 'A', 'B', 'C', 'D', 'MAJ');
#STEP8('state', 'w[5]', '(8*i+5)', 's', 't', 'D', 'A', 'B', 'C', 'MAJ');
#STEP8('state', 'w[6]', '(8*i+6)', 't', 'u', 'C', 'D', 'A', 'B', 'MAJ');
#STEP8('state', 'w[7]', '(8*i+7)', 'u', 'r', 'B', 'C', 'D', 'A', 'MAJ');

####

#STEP8('state', 'IV[0]', 32, 4,  13, 'A', 'B', 'C', 'D', 'IF');
#STEP8('state', 'IV[1]', 33, 13, 10, 'D', 'A', 'B', 'C', 'IF');
#STEP8('state', 'IV[2]', 34, 10, 25, 'C', 'D', 'A', 'B', 'IF');
#STEP8('state', 'IV[3]', 35, 25,  4, 'B', 'C', 'D', 'A', 'IF');

####

#STEP4('state', 'w[0]', '(8*i+0)', 'r', 's', 'A', 'B', 'C', 'D', 'IF');
#STEP4('state', 'w[1]', '(8*i+1)', 's', 't', 'D', 'A', 'B', 'C', 'IF');
#STEP4('state', 'w[2]', '(8*i+2)', 't', 'u', 'C', 'D', 'A', 'B', 'IF');
#STEP4('state', 'w[3]', '(8*i+3)', 'u', 'r', 'B', 'C', 'D', 'A', 'IF');
#STEP4('state', 'w[4]', '(8*i+4)', 'r', 's', 'A', 'B', 'C', 'D', 'MAJ');
#STEP4('state', 'w[5]', '(8*i+5)', 's', 't', 'D', 'A', 'B', 'C', 'MAJ');
#STEP4('state', 'w[6]', '(8*i+6)', 't', 'u', 'C', 'D', 'A', 'B', 'MAJ');
#STEP4('state', 'w[7]', '(8*i+7)', 'u', 'r', 'B', 'C', 'D', 'A', 'MAJ');

####
#STEP4('state', 'IV[0]', 32, 4,  13, 'A', 'B', 'C', 'D', 'IF');
#STEP4('state', 'IV[1]', 33, 13, 10, 'D', 'A', 'B', 'C', 'IF');
#STEP4('state', 'IV[2]', 34, 10, 25, 'C', 'D', 'A', 'B', 'IF');
#STEP4('state', 'IV[3]', 35, 25,  4, 'B', 'C', 'D', 'A', 'IF');
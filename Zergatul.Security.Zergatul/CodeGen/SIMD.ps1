function REDS1($x)
{
    Write-Output "(($x) & 0xFF) - (($x) >> 8)"
}

function REDS2($x)
{
    Write-Output "(($x) & 0xFFFF) + (($x) >> 16)"
}

function FFT_LOOP($params)
{
    $rb = $params[0]
    $hk = $params[1]
    $as = $params[2]
    $id = $params[3]

    Write-Output "do {"

    Write-Output "int u = 0;"
    Write-Output "int v = 0;"
    Write-Output "uint m = q[$rb];"
    Write-Output "uint n = q[$rb + $hk];"
    Write-Output "q[$rb] = m + n;"
    Write-Output "q[$rb + $hk] = m - n;"

    Write-Output "goto id;"
    Write-Output "for (; u < $hk; u += 4, v += 4 * $as)"
    Write-Output "{"
    Write-Output "m = q[$rb + u];"
    Write-Output "n = q[$rb + u + $hk];"
    Write-Output "t = $(REDS2("n * alpha_tab[v]"));"
    Write-Output "q[$rb + u] = m + t;"
    Write-Output "q[$rb + u + $hk] = m - t;"
    Write-Output "id:"
    Write-Output "m = q[$rb + u + 1];"
    Write-Output "n = q[$rb + u + 1 + $hk];"
    Write-Output "}"

    Write-Output "} while (false)"
}

function FFT8($params)
{
    $xb = $params[0]
    $xs = $params[1]
    $d = $params[2]

    Write-Output "do {"
    Write-Output "uint x0 = x[$xb];"
    Write-Output "uint x1 = x[$xb + $xs];"
    Write-Output "uint x2 = x[$xb + 2 * $xs];"
    Write-Output "uint x3 = x[$xb + 3 * $xs];"
    Write-Output "uint a0 = x0 + x2;"
    Write-Output "uint a1 = x0 + (x2 << 4);"
    Write-Output "uint a2 = x0 - x2;"
    Write-Output "uint a3 = x0 - (x2 << 4);"
    Write-Output "uint b0 = x1 + x3;"
    Write-Output "uint b1 = $(REDS1('(x1 << 2) + (x3 << 6)'));"
    Write-Output "uint b2 = (x1 << 4) - (x3 << 4);"
    Write-Output "uint b3 = $(REDS1('(x1 << 6) + (x3 << 2)'));"
    Write-Output "$($d)0 = a0 + b0;"
    Write-Output "$($d)1 = a1 + b1;"
    Write-Output "$($d)2 = a2 + b2;"
    Write-Output "$($d)3 = a3 + b3;"
    Write-Output "$($d)4 = a4 + b4;"
    Write-Output "$($d)5 = a5 + b5;"
    Write-Output "$($d)6 = a6 + b6;"
    Write-Output "$($d)7 = a7 + b7;"
    Write-Output "} while (false)"
}

function FFT16($params)
{
    $xb = $params[0]
    $xs = $params[1]
    $rb = $params[2]

    Write-Output "do {"

    Write-Output "uint d1_0, d1_1, d1_2, d1_3, d1_4, d1_5, d1_6, d1_7;"
    Write-Output "uint d2_0, d2_1, d2_2, d2_3, d2_4, d2_5, d2_6, d2_7;"

    FFT8($xb, "($xs << 1)", 'd1_')
    FFT8("$xb + $xs", "($xs << 1)", 'd2_')

    Write-Output "q[($rb) +  0] = d1_0 + d2_0;       "
    Write-Output "q[($rb) +  1] = d1_1 + (d2_1 << 1);"
    Write-Output "q[($rb) +  2] = d1_2 + (d2_2 << 2);"
    Write-Output "q[($rb) +  3] = d1_3 + (d2_3 << 3);"
    Write-Output "q[($rb) +  4] = d1_4 + (d2_4 << 4);"
    Write-Output "q[($rb) +  5] = d1_5 + (d2_5 << 5);"
    Write-Output "q[($rb) +  6] = d1_6 + (d2_6 << 6);"
    Write-Output "q[($rb) +  7] = d1_7 + (d2_7 << 7);"
    Write-Output "q[($rb) +  8] = d1_0 - d2_0;       "
    Write-Output "q[($rb) +  9] = d1_1 - (d2_1 << 1);"
    Write-Output "q[($rb) + 10] = d1_2 - (d2_2 << 2);"
    Write-Output "q[($rb) + 11] = d1_3 - (d2_3 << 3);"
    Write-Output "q[($rb) + 12] = d1_4 - (d2_4 << 4);"
    Write-Output "q[($rb) + 13] = d1_5 - (d2_5 << 5);"
    Write-Output "q[($rb) + 14] = d1_6 - (d2_6 << 6);"
    Write-Output "q[($rb) + 15] = d1_7 - (d2_7 << 7);"

    Write-Output "} while (false)"
}

function FFT32($params)
{
    $xb = $params[0]
    $xs = $params[1]
    $rb = $params[2]
    $id = $params[3]

    FFT16($xb, "($xs << 1)", $rb)
    FFT16("($xb + $xs)", "($xs << 1)", "$rb + 16")
}

function FFT256($params)
{
    $xb = $params[0]
    $xs = $params[1]
    $rb = $params[2]
    $id = $params[3]


}

function fft64
{
    Write-Output "private static void fft64(object x, ulong xs, uint q)"
    Write-Output "{"
    Write-Output "ulong xd = xs << 1;"
    Write-Output "}"
}

function ProcessBlock512
{

}

Clear-Host
fft64
#ProcessBlock512

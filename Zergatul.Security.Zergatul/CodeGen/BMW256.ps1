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

clear

$sigma = @"
             0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
            14, 10,  4,  8,  9, 15, 13,  6,  1, 12,  0,  2, 11,  7,  5,  3,
            11,  8, 12,  0,  5,  2, 15, 13, 10, 14,  3,  6,  7,  1,  9,  4,
             7,  9,  3,  1, 13, 12, 11, 14,  2,  6,  5, 10,  4,  0, 15,  8,
             9,  0,  5,  7,  2,  4, 10, 15, 14,  1, 11, 12,  6,  8,  3, 13,
             2, 12,  6, 10,  0, 11,  8,  3,  4, 13,  7,  5, 15, 14,  1,  9,
            12,  5,  1, 15, 14, 13,  4, 10,  0,  7,  6,  3,  9,  2,  8, 11,
            13, 11,  7, 14, 12,  1,  3,  9,  5,  0, 15,  4,  8,  6,  2, 10,
             6, 15, 14,  9, 11,  3,  0,  8, 12,  2, 13,  7,  1,  4, 10,  5,
            10,  2,  8,  4,  7,  6,  1,  5, 15, 11,  9, 14,  3, 12, 13,  0,
"@.Replace("`r", '').Replace("`n", '').Replace(' ', '').Split(',')

function s0($x)
{
    return "($x >> 1) ^ ($x << 3) ^ RotateLeft($x, 4) ^ RotateLeft($x, 19)"
}

function s1($x)
{
    return "($x >> 1) ^ ($x << 2) ^ RotateLeft($x, 8) ^ RotateLeft($x, 23)"
}

function s2($x)
{
    return "($x >> 2) ^ ($x << 1) ^ RotateLeft($x, 12) ^ RotateLeft($x, 25)"
}

function s3($x)
{
    return "($x >> 2) ^ ($x << 2) ^ RotateLeft($x, 15) ^ RotateLeft($x, 29)"
}

function s4($x)
{
    return "($x >> 1) ^ $x"
}

function s5($x)
{
    return "($x >> 2) ^ $x"
}

function r1($x)
{
    return "RotateLeft($x, 3)"
}

function r2($x)
{
    return "RotateLeft($x, 7)"
}

function r3($x)
{
    return "RotateLeft($x, 13)"
}

function r4($x)
{
    return "RotateLeft($x, 16)"
}

function r5($x)
{
    return "RotateLeft($x, 19)"
}

function r6($x)
{
    return "RotateLeft($x, 23)"
}

function r7($x)
{
    return "RotateLeft($x, 27)"
}

function Expand1
{
    param
    (
        [string]$ret,
        [int]$j        
    )

    $mp = "RotateLeft(m$(hex($j % 16)), $($j +  1 - 16)) + RotateLeft(m$(hex(($j + 3) % 16)), $($j +  4 - 16)) - RotateLeft(m$(hex(($j + 10) % 16)), $($j +  11 - 16)) + 0x$((hex(($j * 0x05555555), 8)).ToUpper())"

    $x1 = s1("q$(hex(($j - 0x10), 2))")
    $x2 = s2("q$(hex(($j - 0x0f), 2))")
    $x3 = s3("q$(hex(($j - 0x0e), 2))")
    $x4 = s0("q$(hex(($j - 0x0d), 2))")

    $x5 = s1("q$(hex(($j - 0x0c), 2))")
    $x6 = s2("q$(hex(($j - 0x0b), 2))")
    $x7 = s3("q$(hex(($j - 0x0a), 2))")
    $x8 = s0("q$(hex(($j - 0x09), 2))")

    $x9 = s1("q$(hex(($j - 0x08), 2))")
    $x10 = s2("q$(hex(($j - 0x07), 2))")
    $x11 = s3("q$(hex(($j - 0x06), 2))")
    $x12 = s0("q$(hex(($j - 0x05), 2))")

    $x13 = s1("q$(hex(($j - 0x04), 2))")
    $x14 = s2("q$(hex(($j - 0x03), 2))")
    $x15 = s3("q$(hex(($j - 0x02), 2))")
    $x16 = s0("q$(hex(($j - 0x01), 2))")

    Write-Output "uint $ret ="
    Write-Output "    ($x1) +"
    Write-Output "    ($x2) +"
    Write-Output "    ($x3) +"
    Write-Output "    ($x4) +"
    Write-Output "    ($x5) +"
    Write-Output "    ($x6) +"
    Write-Output "    ($x7) +"
    Write-Output "    ($x8) +"
    Write-Output "    ($x9) +"
    Write-Output "    ($x10) +"
    Write-Output "    ($x11) +"
    Write-Output "    ($x12) +"
    Write-Output "    ($x13) +"
    Write-Output "    ($x14) +"
    Write-Output "    ($x15) +"
    Write-Output "    ($x16) +"
    Write-Output "    (h$(hex(($j - 0x09))) ^ ($mp));"
}

function Expand2
{
    param
    (
        [string]$ret,
        [int]$j        
    )

    $mp = "RotateLeft(m$(hex($j % 16)), $(1 + ($j +  2 - 18) % 16)) + RotateLeft(m$(hex(($j + 3) % 16)), $(1 + ($j +  5 - 18) % 16)) - RotateLeft(m$(hex(($j + 10) % 16)), $(1 + ($j +  12 - 18) % 16)) + 0x$((hex(($j * 0x05555555), 8)).ToUpper())"

    $x1 = "q$(hex(($j - 0x10), 2))"
    $x2 = r1("q$(hex(($j - 0x0f), 2))")
    $x3 = "q$(hex(($j - 0x0e), 2))"
    $x4 = r2("q$(hex(($j - 0x0d), 2))")

    $x5 = "q$(hex(($j - 0x0c), 2))"
    $x6 = r3("q$(hex(($j - 0x0b), 2))")
    $x7 = "q$(hex(($j - 0x0a), 2))"
    $x8 = r4("q$(hex(($j - 0x09), 2))")

    $x9 = "q$(hex(($j - 0x08), 2))"
    $x10 = r5("q$(hex(($j - 0x07), 2))")
    $x11 = "q$(hex(($j - 0x06), 2))"
    $x12 = r6("q$(hex(($j - 0x05), 2))")

    $x13 = "q$(hex(($j - 0x04), 2))"
    $x14 = r7("q$(hex(($j - 0x03), 2))")
    $x15 = s4("q$(hex(($j - 0x02), 2))")
    $x16 = s5("q$(hex(($j - 0x01), 2))")

    Write-Output "uint $ret ="
    Write-Output "    $x1 + $x2 + $x3 + $x4 +"
    Write-Output "    $x5 + $x6 + $x7 + $x8 +"
    Write-Output "    $x9 + $x10 + $x11 + $x12 +"
    Write-Output "    $x13 + $x14 + ($x15) + ($x16) +"
    Write-Output "    (h$(hex((($j - 0x09) % 16))) ^ ($mp));"
}

function F0()
{
    Write-Output "uint q00 = (m5 ^ h5) - (m7 ^ h7) + (ma ^ ha) + (md ^ hd) + (me ^ he);"
    Write-Output "q00 = ($(s0('q00'))) + h1;"
    Write-Output "uint q01 = (m6 ^ h6) - (m8 ^ h8) + (mb ^ hb) + (me ^ he) - (mf ^ hf);"
    Write-Output "q01 = ($(s1('q01'))) + h2;"
    Write-Output "uint q02 = (m0 ^ h0) + (m7 ^ h7) + (m9 ^ h9) - (mc ^ hc) + (mf ^ hf);"
    Write-Output "q02 = ($(s2('q02'))) + h3;"
    Write-Output "uint q03 = (m0 ^ h0) - (m1 ^ h1) + (m8 ^ h8) - (ma ^ ha) + (md ^ hd);"
    Write-Output "q03 = ($(s3('q03'))) + h4;"
    Write-Output "uint q04 = (m1 ^ h1) + (m2 ^ h2) + (m9 ^ h9) - (mb ^ hb) - (me ^ he);"
    Write-Output "q04 = ($(s4('q04'))) + h5;"
    Write-Output "uint q05 = (m3 ^ h3) - (m2 ^ h2) + (ma ^ ha) - (mc ^ hc) + (mf ^ hf);"
    Write-Output "q05 = ($(s0('q05'))) + h6;"
    Write-Output "uint q06 = (m4 ^ h4) - (m0 ^ h0) - (m3 ^ h3) - (mb ^ hb) + (md ^ hd);"
    Write-Output "q06 = ($(s1('q06'))) + h7;"
    Write-Output "uint q07 = (m1 ^ h1) - (m4 ^ h4) - (m5 ^ h5) - (mc ^ hc) - (me ^ he);"
    Write-Output "q07 = ($(s2('q07'))) + h8;"
    Write-Output "uint q08 = (m2 ^ h2) - (m5 ^ h5) - (m6 ^ h6) + (md ^ hd) - (mf ^ hf);"
    Write-Output "q08 = ($(s3('q08'))) + h9;"
    Write-Output "uint q09 = (m0 ^ h0) - (m3 ^ h3) + (m6 ^ h6) - (m7 ^ h7) + (me ^ he);"
    Write-Output "q09 = ($(s4('q09'))) + ha;"
    Write-Output "uint q0a = (m8 ^ h8) - (m1 ^ h1) - (m4 ^ h4) - (m7 ^ h7) + (mf ^ hf);"
    Write-Output "q0a = ($(s0('q0a'))) + hb;"
    Write-Output "uint q0b = (m8 ^ h8) - (m0 ^ h0) - (m2 ^ h2) - (m5 ^ h5) + (m9 ^ h9);"
    Write-Output "q0b = ($(s1('q0b'))) + hc;"
    Write-Output "uint q0c = (m1 ^ h1) + (m3 ^ h3) - (m6 ^ h6) - (m9 ^ h9) + (ma ^ ha);"
    Write-Output "q0c = ($(s2('q0c'))) + hd;"
    Write-Output "uint q0d = (m2 ^ h2) + (m4 ^ h4) + (m7 ^ h7) + (ma ^ ha) + (mb ^ hb);"
    Write-Output "q0d = ($(s3('q0d'))) + he;"
    Write-Output "uint q0e = (m3 ^ h3) - (m5 ^ h5) + (m8 ^ h8) - (mb ^ hb) - (mc ^ hc);"
    Write-Output "q0e = ($(s4('q0e'))) + hf;"
    Write-Output "uint q0f = (mc ^ hc) - (m4 ^ h4) - (m6 ^ h6) - (m9 ^ h9) + (md ^ hd);"
    Write-Output "q0f = ($(s0('q0f'))) + h0;"
}

function F1()
{
    Expand1 -ret "q10" -j 16
    Expand1 -ret "q11" -j 17
    Expand2 -ret "q12" -j 18
    Expand2 -ret "q13" -j 19
    Expand2 -ret "q14" -j 20
    Expand2 -ret "q15" -j 21
    Expand2 -ret "q16" -j 22
    Expand2 -ret "q17" -j 23
    Expand2 -ret "q18" -j 24
    Expand2 -ret "q19" -j 25
    Expand2 -ret "q1a" -j 26
    Expand2 -ret "q1b" -j 27
    Expand2 -ret "q1c" -j 28
    Expand2 -ret "q1d" -j 29
    Expand2 -ret "q1e" -j 30
    Expand2 -ret "q1f" -j 31
}

function F2()
{
    Write-Output "uint xl = q10 ^ q11 ^ q12 ^ q13 ^ q14 ^ q15 ^ q16 ^ q17;"
    Write-Output "uint xh = q18 ^ q19 ^ q1a ^ q1b ^ q1c ^ q1d ^ q1e ^ q1f ^ xl;"
    Write-Output "h0 = ((xh <<  5) ^ (q10 >>  5) ^ m0) + (xl ^ q18 ^ q0);"
    Write-Output "h1 = ((xh >>  7) ^ (q11 <<  8) ^ m1) + (xl ^ q19 ^ q1);"
    Write-Output "h2 = ((xh >>  5) ^ (q12 <<  5) ^ m2) + (xl ^ q1a ^ q2);"
    Write-Output "h3 = ((xh >>  1) ^ (q13 <<  5) ^ m3) + (xl ^ q1b ^ q3);"
    Write-Output "h4 = ((xh >>  3) ^ (q14 <<  0) ^ m4) + (xl ^ q1c ^ q4);"
    Write-Output "h5 = ((xh <<  6) ^ (q15 >>  6) ^ m5) + (xl ^ q1d ^ q5);"
    Write-Output "h6 = ((xh >>  4) ^ (q16 <<  6) ^ m6) + (xl ^ q1e ^ q6);"
    Write-Output "h7 = ((xh >> 11) ^ (q17 <<  2) ^ m7) + (xl ^ q1f ^ q7);"
    Write-Output "h8 = RotateLeft(h4,  9) + (xh ^ q18 ^ m8) + ((xl << 8) ^ q17 ^ q08);"
    Write-Output "h9 = RotateLeft(h5, 10) + (xh ^ q19 ^ m9) + ((xl >> 6) ^ q10 ^ q09);"
    Write-Output "ha = RotateLeft(h6, 11) + (xh ^ q1a ^ ma) + ((xl << 6) ^ q11 ^ q0a);"
    Write-Output "hb = RotateLeft(h7, 12) + (xh ^ q1b ^ mb) + ((xl << 4) ^ q12 ^ q0b);"
    Write-Output "hc = RotateLeft(h0, 13) + (xh ^ q1c ^ mc) + ((xl >> 3) ^ q13 ^ q0c);"
    Write-Output "hd = RotateLeft(h1, 14) + (xh ^ q1d ^ md) + ((xl >> 4) ^ q14 ^ q0d);"
    Write-Output "he = RotateLeft(h2, 15) + (xh ^ q1e ^ me) + ((xl >> 7) ^ q15 ^ q0e);"
    Write-Output "hf = RotateLeft(h3, 16) + (xh ^ q1f ^ mf) + ((xl >> 2) ^ q16 ^ q0f);"
}

F0
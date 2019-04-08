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

function F($params)
{
    $x = $params[0]
    $y = $params[1]
    $z = $params[2]

    return "($x & $y) | (~$x & $z)"
}

function G($params)
{
    $x = $params[0]
    $y = $params[1]
    $z = $params[2]

    return "($x & $y) | ($x & $z) | ($y & $z)"
}

function Hm($params)
{
    $x = $params[0]
    $y = $params[1]
    $z = $params[2]

    return "$x ^ $y ^ $z"
}

function FF($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]
    $x = $params[4]
    $s = $params[5]

    Write-Output "$a = RotateLeft($a + ($(F($b, $c, $d))) + $x, 0x$(hex($s, 2)));"
}

function GG($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]
    $x = $params[4]
    $s = $params[5]

    Write-Output "$a = RotateLeft($a + ($(G($b, $c, $d))) + $x + 0x5A827999, 0x$(hex($s, 2)));"
}


function HH($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]
    $x = $params[4]
    $s = $params[5]

    Write-Output "$a = RotateLeft($a + ($(Hm($b, $c, $d))) + $x + 0x6ED9EBA1, 0x$(hex($s, 2)));"
}

function m($i)
{
    return "m$(hex($i, 1))"
}

$S11 = 3
$S12 = 7
$S13 = 11
$S14 = 19
$S21 = 3
$S22 = 5
$S23 = 9
$S24 = 13
$S31 = 3
$S32 = 9
$S33 = 11
$S34 = 15

Write-Output ""
Write-Output "// Round 1"
FF('a', 'b', 'c', 'd', (m(0)), $S11)
FF('d', 'a', 'b', 'c', (m(1)), $S12)
FF('c', 'd', 'a', 'b', (m(2)), $S13)
FF('b', 'c', 'd', 'a', (m(3)), $S14)
FF('a', 'b', 'c', 'd', (m(4)), $S11)
FF('d', 'a', 'b', 'c', (m(5)), $S12)
FF('c', 'd', 'a', 'b', (m(6)), $S13)
FF('b', 'c', 'd', 'a', (m(7)), $S14)
FF('a', 'b', 'c', 'd', (m(8)), $S11)
FF('d', 'a', 'b', 'c', (m(9)), $S12)
FF('c', 'd', 'a', 'b', (m(10)), $S13)
FF('b', 'c', 'd', 'a', (m(11)), $S14)
FF('a', 'b', 'c', 'd', (m(12)), $S11)
FF('d', 'a', 'b', 'c', (m(13)), $S12)
FF('c', 'd', 'a', 'b', (m(14)), $S13)
FF('b', 'c', 'd', 'a', (m(15)), $S14)

Write-Output ""
Write-Output "// Round 2"
GG('a', 'b', 'c', 'd', (m(0)), $S21)
GG('d', 'a', 'b', 'c', (m(4)), $S22)
GG('c', 'd', 'a', 'b', (m(8)), $S23)
GG('b', 'c', 'd', 'a', (m(12)), $S24)
GG('a', 'b', 'c', 'd', (m(1)), $S21)
GG('d', 'a', 'b', 'c', (m(5)), $S22)
GG('c', 'd', 'a', 'b', (m(9)), $S23)
GG('b', 'c', 'd', 'a', (m(13)), $S24)
GG('a', 'b', 'c', 'd', (m(2)), $S21)
GG('d', 'a', 'b', 'c', (m(6)), $S22)
GG('c', 'd', 'a', 'b', (m(10)), $S23)
GG('b', 'c', 'd', 'a', (m(14)), $S24)
GG('a', 'b', 'c', 'd', (m(3)), $S21)
GG('d', 'a', 'b', 'c', (m(7)), $S22)
GG('c', 'd', 'a', 'b', (m(11)), $S23)
GG('b', 'c', 'd', 'a', (m(15)), $S24)

Write-Output ""
Write-Output "// Round 3"
HH('a', 'b', 'c', 'd', (m(0)), $S31)
HH('d', 'a', 'b', 'c', (m(8)), $S32)
HH('c', 'd', 'a', 'b', (m(4)), $S33)
HH('b', 'c', 'd', 'a', (m(12)), $S34)
HH('a', 'b', 'c', 'd', (m(2)), $S31)
HH('d', 'a', 'b', 'c', (m(10)), $S32)
HH('c', 'd', 'a', 'b', (m(6)), $S33)
HH('b', 'c', 'd', 'a', (m(14)), $S34)
HH('a', 'b', 'c', 'd', (m(1)), $S31)
HH('d', 'a', 'b', 'c', (m(9)), $S32)
HH('c', 'd', 'a', 'b', (m(5)), $S33)
HH('b', 'c', 'd', 'a', (m(13)), $S34)
HH('a', 'b', 'c', 'd', (m(3)), $S31)
HH('d', 'a', 'b', 'c', (m(11)), $S32)
HH('c', 'd', 'a', 'b', (m(7)), $S33)
HH('b', 'c', 'd', 'a', (m(15)), $S34)
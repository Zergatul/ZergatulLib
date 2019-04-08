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

function RSTT($params)
{
    $d = $params[0]
    $a = $params[1]
    $b0 = $params[2]
    $b1 = $params[3]
    $b2 = $params[4]
    $b3 = $params[5]
    $b4 = $params[6]
    $b5 = $params[7]
    $b6 = $params[8]
    $b7 = $params[9]

    Write-Output "ulong t$d ="
    Write-Output "    T0[($a$b0 >> 0x00) & 0xFF] ^ T1[($a$b1 >> 0x08) & 0xFF] ^ T2[($a$b2 >> 0x10) & 0xFF] ^ T3[($a$b3 >> 0x18) & 0xFF] ^"
    Write-Output "    T4[($a$b4 >> 0x20) & 0xFF] ^ T5[($a$b5 >> 0x28) & 0xFF] ^ T6[($a$b6 >> 0x30) & 0xFF] ^ T7[($a$b7 >> 0x38) & 0xFF];"
}

function P
{
    param($a, $r)

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "$a$i ^= 0x$((hex(($i * 16), 2)).ToUpper()) + $r;"
    }

    RSTT(0, $a, 0, 1, 2, 3, 4, 5, 6, 7)
    RSTT(1, $a, 1, 2, 3, 4, 5, 6, 7, 0)
    RSTT(2, $a, 2, 3, 4, 5, 6, 7, 0, 1)
    RSTT(3, $a, 3, 4, 5, 6, 7, 0, 1, 2)
    RSTT(4, $a, 4, 5, 6, 7, 0, 1, 2, 3)
    RSTT(5, $a, 5, 6, 7, 0, 1, 2, 3, 4)
    RSTT(6, $a, 6, 7, 0, 1, 2, 3, 4, 5)
    RSTT(7, $a, 7, 0, 1, 2, 3, 4, 5, 6)

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "$a$i = t$i;"
    }
}

function Q
{
    param($a, $r)

    for ($i = 0; $i -lt 8; $i++)
    {
        $precalc = (hex(((-bnot ($i * 16)) -band 0xFF), 2)).ToUpper()
        Write-Output "$a$(hex($i, 1)) ^= ($r << 56) ^ 0x$($precalc)FFFFFFFFFFFFFF;"
    }

    RSTT(0, $a, 1, 3, 5, 7, 0, 2, 4, 6)
    RSTT(1, $a, 2, 4, 6, 0, 1, 3, 5, 7)
    RSTT(2, $a, 3, 5, 7, 1, 2, 4, 6, 0)
    RSTT(3, $a, 4, 6, 0, 2, 3, 5, 7, 1)
    RSTT(4, $a, 5, 7, 1, 3, 4, 6, 0, 2)
    RSTT(5, $a, 6, 0, 2, 4, 5, 7, 1, 3)
    RSTT(6, $a, 7, 1, 3, 5, 6, 0, 2, 4)
    RSTT(7, $a, 0, 2, 4, 6, 7, 1, 3, 5)

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "$a$i = t$i;"
    }
}

function ProcessBlock
{
    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "ulong m$i = ToUInt64(buffer, 0x$(hex(($i * 8), 2)), ByteOrder.LittleEndian);"
    }

    Write-Output ""

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "ulong g$i = m$i ^ s$i;"
    }

    Write-Output ""
    Write-Output "#region P(g)"
    Write-Output ""

    Write-Output "for (ulong r = 0; r < 10; r++)"
    Write-Output "{"
    P -a 'g' -r 'r'
    Write-Output "}"

    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""
    Write-Output "#region Q(m)"
    Write-Output ""

    Write-Output "for (ulong r = 0; r < 10; r++)"
    Write-Output "{"
    Q -a 'm' -r 'r'
    Write-Output "}"

    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "s$i ^= g$i ^ m$i;"
    }

    Write-Output ""
    Write-Output "blocks++;"
}

function LastRound
{
    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "ulong x$i = s$i;"
    }

    Write-Output ""
    Write-Output "#region P(x)"
    Write-Output ""

    Write-Output "for (ulong r = 0; r < 10; r++)"
    Write-Output "{"
    P -a 'x' -r 'r'
    Write-Output "}"

    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "s$i ^= x$i;"
    }
}

#ProcessBlock
 LastRound
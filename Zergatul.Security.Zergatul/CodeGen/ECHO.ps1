function hex($params)
{
    $i = $params[0]
    $digits = $params[1]
    if (-not $digits)
    {
        $digits = 1
    }
    return [System.Convert]::ToString([int]$i, 16).PadLeft($digits, '0')
}

function AES_ROUND_LE($params)
{
    $X0 = $params[0]
    $X1 = $params[1]
    $X2 = $params[2]
    $X3 = $params[3]
    $K0 = $params[4]
    $K1 = $params[5]
    $K2 = $params[6]
    $K3 = $params[7]
    $Y0 = $params[8]
    $Y1 = $params[9]
    $Y2 = $params[10]
    $Y3 = $params[11]

    if ($K0 -eq 0)
    {
        $ending = ""
    }
    else
    {
        $ending = "^ $K0"
    }
    Write-Output "$Y0 = AES0[$X0 & 0xFF] ^ AES1[($X1 >> 8) & 0xFF] ^ AES2[($X2 >> 16) & 0xFF] ^ AES3[($X3 >> 24) & 0xFF]$ending;"

    if ($K1 -eq 0)
    {
        $ending = ""
    }
    else
    {
        $ending = "^ $K1"
    }
    Write-Output "$Y1 = AES0[$X1 & 0xFF] ^ AES1[($X2 >> 8) & 0xFF] ^ AES2[($X3 >> 16) & 0xFF] ^ AES3[($X0 >> 24) & 0xFF]$ending;"

    if ($K2 -eq 0)
    {
        $ending = ""
    }
    else
    {
        $ending = "^ $K2"
    }
    Write-Output "$Y2 = AES0[$X2 & 0xFF] ^ AES1[($X3 >> 8) & 0xFF] ^ AES2[($X0 >> 16) & 0xFF] ^ AES3[($X1 >> 24) & 0xFF]$ending;"

    if ($K3 -eq 0)
    {
        $ending = ""
    }
    else
    {
        $ending = "^ $K3"
    }
    Write-Output "$Y3 = AES0[$X3 & 0xFF] ^ AES1[($X0 >> 8) & 0xFF] ^ AES2[($X1 >> 16) & 0xFF] ^ AES3[($X2 >> 24) & 0xFF]$ending;"
}

function AES_ROUND_NOKEY_LE($params)
{
    $X0 = $params[0]
    $X1 = $params[1]
    $X2 = $params[2]
    $X3 = $params[3]
    $Y0 = $params[4]
    $Y1 = $params[5]
    $Y2 = $params[6]
    $Y3 = $params[7]

    AES_ROUND_LE($X0, $X1, $X2, $X3, 0, 0, 0, 0, $Y0, $Y1, $Y2, $Y3)
}

function AES_ROUND_NOKEY($params)
{
    $x0 = $params[0]
    $x1 = $params[1]
    $x2 = $params[2]
    $x3 = $params[3]

    Write-Output "t0 = $x0;"
    Write-Output "t1 = $x1;"
    Write-Output "t2 = $x2;"
    Write-Output "t3 = $x3;"
    AES_ROUND_NOKEY_LE('t0', 't1', 't2', 't3', $x0, $x1, $x2, $x3)
}

function AES_2ROUNDS($i)
{
    $i = $i * 2

    Write-Output "x0 = (uint)w$(hex(($i + 0), 2));"
    Write-Output "x1 = (uint)(w$(hex(($i + 0), 2)) >> 32);"
    Write-Output "x2 = (uint)w$(hex(($i + 1), 2));"
    Write-Output "x3 = (uint)(w$(hex(($i + 1), 2)) >> 32);"

    AES_ROUND_LE('x0', 'x1', 'x2', 'x3', 'k0', 'k1', 'k2', 'k3', 'y0', 'y1', 'y2', 'y3')
    AES_ROUND_NOKEY_LE('y0', 'y1', 'y2', 'y3', 'x0', 'x1', 'x2', 'x3')

    Write-Output "w$(hex(($i + 0), 2)) = (ulong)x0 | ((ulong)x1 << 32);"
    Write-Output "w$(hex(($i + 1), 2)) = (ulong)x2 | ((ulong)x3 << 32);"

    Write-Output "if (++k0 == 0)"
    Write-Output "if (++k1 == 0)"
    Write-Output "if (++k2 == 0)"
    Write-Output "k3++;"
}

function SHIFT_ROW1($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]

    Write-Output "tmp = w$(hex(($a * 2), 2));"
    Write-Output "w$(hex(($a * 2), 2)) = w$(hex(($b * 2), 2));"
    Write-Output "w$(hex(($b * 2), 2)) = w$(hex(($c * 2), 2));"
    Write-Output "w$(hex(($c * 2), 2)) = w$(hex(($d * 2), 2));"
    Write-Output "w$(hex(($d * 2), 2)) = tmp;"

    Write-Output "tmp = w$(hex(($a * 2 + 1), 2));"
    Write-Output "w$(hex(($a * 2 + 1), 2)) = w$(hex(($b * 2 + 1), 2));"
    Write-Output "w$(hex(($b * 2 + 1), 2)) = w$(hex(($c * 2 + 1), 2));"
    Write-Output "w$(hex(($c * 2 + 1), 2)) = w$(hex(($d * 2 + 1), 2));"
    Write-Output "w$(hex(($d * 2 + 1), 2)) = tmp;"
}

function SHIFT_ROW2($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]

    Write-Output "tmp = w$(hex(($a * 2), 2));"
    Write-Output "w$(hex(($a * 2), 2)) = w$(hex(($c * 2), 2));"
    Write-Output "w$(hex(($c * 2), 2)) = tmp;"

    Write-Output "tmp = w$(hex(($b * 2), 2));"
    Write-Output "w$(hex(($b * 2), 2)) = w$(hex(($d * 2), 2));"
    Write-Output "w$(hex(($d * 2), 2)) = tmp;"

    Write-Output "tmp = w$(hex(($a * 2 + 1), 2));"
    Write-Output "w$(hex(($a * 2 + 1), 2)) = w$(hex(($c * 2 + 1), 2));"
    Write-Output "w$(hex(($c * 2 + 1), 2)) = tmp;"

    Write-Output "tmp = w$(hex(($b * 2 + 1), 2));"
    Write-Output "w$(hex(($b * 2 + 1), 2)) = w$(hex(($d * 2 + 1), 2));"
    Write-Output "w$(hex(($d * 2 + 1), 2)) = tmp;"
}

function SHIFT_ROW3($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]

    SHIFT_ROW1($d, $c, $b, $a)
}

function MIX_COLUMN1($params)
{
    $ia = $params[0]
    $ib = $params[1]
    $ic = $params[2]
    $id = $params[3]
    $n  = $params[4]

    Write-Output "a = w$(hex((2 * $ia + $n), 2));"
    Write-Output "b = w$(hex((2 * $ib + $n), 2));"
    Write-Output "c = w$(hex((2 * $ic + $n), 2));"
    Write-Output "d = w$(hex((2 * $id + $n), 2));"

    Write-Output "ab = a ^ b;"
    Write-Output "bc = b ^ c;"
    Write-Output "cd = c ^ d;"

    Write-Output "abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);"
    Write-Output "bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);"
    Write-Output "cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);"

    Write-Output "w$(hex((2 * $ia + $n), 2)) = abx ^ bc ^ d;"
    Write-Output "w$(hex((2 * $ib + $n), 2)) = bcx ^ a ^ cd;"
    Write-Output "w$(hex((2 * $ic + $n), 2)) = cdx ^ ab ^ d;"
    Write-Output "w$(hex((2 * $id + $n), 2)) = abx ^ bcx ^ cdx ^ ab ^ c;"
}

function MIX_COLUMN($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]

    MIX_COLUMN1($a, $b, $c, $d, 0)
    MIX_COLUMN1($a, $b, $c, $d, 1)
}

function Round
{
    Write-Output "#region Sub Words"
    Write-Output ""
    AES_2ROUNDS( 0)
    AES_2ROUNDS( 1)
    AES_2ROUNDS( 2)
    AES_2ROUNDS( 3)
    AES_2ROUNDS( 4)
    AES_2ROUNDS( 5)
    AES_2ROUNDS( 6)
    AES_2ROUNDS( 7)
    AES_2ROUNDS( 8)
    AES_2ROUNDS( 9)
    AES_2ROUNDS(10)
    AES_2ROUNDS(11)
    AES_2ROUNDS(12)
    AES_2ROUNDS(13)
    AES_2ROUNDS(14)
    AES_2ROUNDS(15)
    Write-Output ""
    Write-Output "#endregion"
    Write-Output ""

    Write-Output "#region Shift Rows"
    Write-Output ""
    SHIFT_ROW1(1, 5, 9, 13)
    SHIFT_ROW2(2, 6, 10, 14)
    SHIFT_ROW3(3, 7, 11, 15)
    Write-Output ""
    Write-Output "#endregion"
    Write-Output ""

    Write-Output "#region Mix Columns"
    Write-Output ""
    MIX_COLUMN(0, 1, 2, 3)
    MIX_COLUMN(4, 5, 6, 7)
    MIX_COLUMN(8, 9, 10, 11)
    MIX_COLUMN(12, 13, 14, 15)
    Write-Output ""
    Write-Output "#endregion"
}

function ProcessBlock512
{
    for ($i = 0; $i -lt 16; $i++)
    {
        Write-Output "ulong w$(hex($i, 2)) = v$(hex($i));"
    }

    Write-Output ""

    for ($i = 0; $i -lt 16; $i++)
    {
        Write-Output "ulong w$(hex(($i + 16), 2)) = ToUInt64(buffer, 0x$(hex(($i * 8), 2)), ByteOrder.LittleEndian);"
    }

    Write-Output ""

    Write-Output "uint k0 = c0;"
    Write-Output "uint k1 = c1;"
    Write-Output "uint k2 = c2;"
    Write-Output "uint k3 = c3;"

    Write-Output ""

    Write-Output "uint x0, x1, x2, x3;"
    Write-Output "uint y0, y1, y2, y3;"
    Write-Output "ulong tmp;"
    Write-Output "ulong a, b, c, d, ab, bc, cd, abx, bcx, cdx;"

    Write-Output ""
    Write-Output "for (int r = 0; r < 10; r++)"
    Write-Output "{"
    Round
    Write-Output "}"
    Write-Output ""

    for ($i = 0; $i -lt 16; $i++)
    {
        Write-Output "v$(hex($i)) ^= ToUInt64(buffer, 0x$(hex(($i * 8), 2)), ByteOrder.LittleEndian) ^ w$(hex($i, 2)) ^ w$(hex(($i + 16), 2));"
    }
}

function ProcessBlock256
{
    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "ulong w$(hex($i, 2)) = v$(hex($i));"
    }

    Write-Output ""

    for ($i = 0; $i -lt 24; $i++)
    {
        Write-Output "ulong w$(hex(($i + 8), 2)) = ToUInt64(buffer, 0x$(hex(($i * 8), 2)), ByteOrder.LittleEndian);"
    }

    Write-Output ""

    Write-Output "uint k0 = c0;"
    Write-Output "uint k1 = c1;"
    Write-Output "uint k2 = c2;"
    Write-Output "uint k3 = c3;"

    Write-Output ""

    Write-Output "uint x0, x1, x2, x3;"
    Write-Output "uint y0, y1, y2, y3;"
    Write-Output "ulong tmp;"
    Write-Output "ulong a, b, c, d, ab, bc, cd, abx, bcx, cdx;"

    Write-Output ""
    Write-Output "for (int r = 0; r < 8; r++)"
    Write-Output "{"
    Round
    Write-Output "}"
    Write-Output ""

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "v$(hex($i)) ^="
        Write-Output "    ToUInt64(buffer, 0x$(hex(($i * 8), 2)), ByteOrder.LittleEndian) ^"
        Write-Output "    ToUInt64(buffer, 0x$(hex(($i * 8 + 64), 2)), ByteOrder.LittleEndian) ^"
        Write-Output "    ToUInt64(buffer, 0x$(hex(($i * 8 + 128), 2)), ByteOrder.LittleEndian) ^"
        Write-Output "    w$(hex($i, 2)) ^ w$(hex(($i + 8), 2)) ^ w$(hex(($i + 16), 2)) ^ w$(hex(($i + 24), 2));"
    }
}

clear

ProcessBlock256
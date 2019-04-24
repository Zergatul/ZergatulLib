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

function State
{
    for ($i = 0; $i -lt 5; $i++)
    {
        $s = ""
        for ($j = 0; $j -lt 8; $j++)
        {
            $s += "h$i$j, "
        }
        Write-Output "uint $($s.Substring(0, $s.Length - 2));"
    }    
}

function Reset
{
    $init = @(
        '0x6D251E69'
        '0x44B051E0'
        '0x4EAA6FB4'
        '0xDBF78465'
        '0x6E292011'
        '0x90152DF4'
        '0xEE058139'
        '0xDEF610BB'
        '0xC3B44B95' 
        '0xD9D2F256'
        '0x70EEE9A0' 
        '0xDE099FA3'
        '0x5D9B0557' 
        '0x8FC944B3'
        '0xCF1CCF0E' 
        '0x746CD581'
        '0xF7EFC89D' 
        '0x5DBA5781'
        '0x04016CE5' 
        '0xAD659C05'
        '0x0306194F'
        '0x666D1836'
        '0x24AA230A' 
        '0x8B264AE7'
        '0x858075D5' 
        '0x36D79CCE'
        '0xE571F7D7' 
        '0x204B1F67'
        '0x35870C6A' 
        '0x57E9E923'
        '0x14BCB808'
        '0x7CDE72CE'
        '0x6C68E9BE' 
        '0x5EC41E22'
        '0xC825B7C7' 
        '0xAFFB4363'
        '0xF5DF3999' 
        '0x0FC688F1'
        '0xB07224CC' 
        '0x03E86CEA'
    )

    for ([int]$i = 0; $i -lt 40; $i++)
    {
        Write-Output "h$([Math]::Floor($i / 8))$($i % 8) = $($init[$i]);"
    }
}

function XOR($params)
{
    $d = $params[0]
    $s1 = $params[1]
    $s2 = $params[2]

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "$d$i = $s1$i ^ $s2$i;"
    }
}

function M2($params)
{
    $d = $params[0]
    $s = $params[1]

    Write-Output "tmp = $($s)7;"
    Write-Output "$($d)7 = $($s)6;"
    Write-Output "$($d)6 = $($s)5;"
    Write-Output "$($d)5 = $($s)4;"
    Write-Output "$($d)4 = $($s)3 ^ tmp;"
    Write-Output "$($d)3 = $($s)2 ^ tmp;"
    Write-Output "$($d)2 = $($s)1;"
    Write-Output "$($d)1 = $($s)0 ^ tmp;"
    Write-Output "$($d)0 = tmp;"
}

function MI5
{
    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "uint m$i = ToUInt32(buffer, 0x$((hex(($i * 4), 2)).ToUpper()), ByteOrder.BigEndian);"
    }

    Write-Output ""

    Write-Output "uint a0, a1, a2, a3, a4, a5, a6, a7;"
    Write-Output "uint b0, b1, b2, b3, b4, b5, b6, b7;"
    Write-Output "uint tmp;"

    Write-Output ""

    XOR('a', 'v0', 'v1')
    XOR('b', 'v2', 'v3')
    XOR('a', 'a', 'b')
    XOR('a', 'a', 'v4')
    M2('a', 'a')
    XOR('v0', 'a', 'v0')
    XOR('v1', 'a', 'v1')
    XOR('v2', 'a', 'v2')
    XOR('v3', 'a', 'v3')
    XOR('v4', 'a', 'v4')
    M2('b', 'v0')
    XOR('b', 'b', 'v1')
	M2('v1', 'v1')
	XOR('v1', 'v1', 'v2')
	M2('v2', 'v2')
	XOR('v2', 'v2', 'v3')
	M2('v3', 'v3')
	XOR('v3', 'v3', 'v4')
	M2('v4', 'v4')
	XOR('v4', 'v4', 'v0')
	M2('v0', 'b')
	XOR('v0', 'v0', 'v4')
	M2('v4', 'v4')
	XOR('v4', 'v4', 'v3')
	M2('v3', 'v3')
	XOR('v3', 'v3', 'v2')
	M2('v2', 'v2')
	XOR('v2', 'v2', 'v1')
	M2('v1', 'v1')
	XOR('v1', 'v1', 'b')
	XOR('v0', 'v0', 'm')
	M2('m', 'm')
	XOR('v1', 'v1', 'm')
	M2('m', 'm')
	XOR('v2', 'v2', 'm')
	M2('m', 'm')
	XOR('v3', 'v3', 'm')
	M2('m', 'm')
	XOR('v4', 'v4', 'm')
}

function TWEAK5
{
    Write-Output "v14 = RotateLeft(v14, 1);"
    Write-Output "v15 = RotateLeft(v15, 1);"
    Write-Output "v16 = RotateLeft(v16, 1);"
    Write-Output "v17 = RotateLeft(v17, 1);"
    Write-Output "v24 = RotateLeft(v24, 2);"
    Write-Output "v25 = RotateLeft(v25, 2);"
    Write-Output "v26 = RotateLeft(v26, 2);"
    Write-Output "v27 = RotateLeft(v27, 2);"
    Write-Output "v34 = RotateLeft(v34, 3);"
    Write-Output "v35 = RotateLeft(v35, 3);"
    Write-Output "v36 = RotateLeft(v36, 3);"
    Write-Output "v37 = RotateLeft(v37, 3);"
    Write-Output "v44 = RotateLeft(v44, 4);"
    Write-Output "v45 = RotateLeft(v45, 4);"
    Write-Output "v46 = RotateLeft(v46, 4);"
    Write-Output "v47 = RotateLeft(v47, 4);"
}

function SUB_CRUMB_GEN($params)
{
    $a0 = $params[0]
    $a1 = $params[1]
    $a2 = $params[2]
    $a3 = $params[3]
    $w = $params[4]

    if ($w -eq 64)
    {
        $tmp = "ltmp";
    }
    else
    {
        $tmp = "tmp"
    }

    Write-Output "$tmp = $a0;"
    Write-Output "$a0 |= $a1;"
    Write-Output "$a2 ^= $a3;"
    Write-Output "$a1 = ~$a1;"
    Write-Output "$a0 ^= $a3;"
    Write-Output "$a3 &= $tmp;"
    Write-Output "$a1 ^= $a3;"
    Write-Output "$a3 ^= $a2;"
    Write-Output "$a2 &= $a0;"
    Write-Output "$a0 = ~$a0;"
    Write-Output "$a2 ^= $a1;"
    Write-Output "$a1 |= $a3;"
    Write-Output "$tmp ^= $a1;"
    Write-Output "$a3 ^= $a2;"
    Write-Output "$a2 &= $a1;"
    Write-Output "$a1 ^= $a0;"
    Write-Output "$a0 = $tmp;"
}

function SUB_CRUMB($params)
{
    $a0 = $params[0]
    $a1 = $params[1]
    $a2 = $params[2]
    $a3 = $params[3]
    
    SUB_CRUMB_GEN($a0, $a1, $a2, $a3, 32)
}

function SUB_CRUMBW($params)
{
    $a0 = $params[0]
    $a1 = $params[1]
    $a2 = $params[2]
    $a3 = $params[3]

    SUB_CRUMB_GEN($a0, $a1, $a2, $a3, 64)
}

function MIX_WORDW($params)
{
    $u = $params[0]
    $v = $params[1]

    Write-Output "$v ^= $u;"
    Write-Output "ul = (uint)$u;"
    Write-Output "uh = (uint)($u >> 32);"
    Write-Output "vl = (uint)$v;"
    Write-Output "vh = (uint)($v >> 32);"
    Write-Output "ul = RotateLeft(ul, 2) ^ vl;"
    Write-Output "vl = RotateLeft(vl, 14) ^ ul;"
    Write-Output "ul = RotateLeft(ul, 10) ^ vl;"
    Write-Output "vl = RotateLeft(vl, 1);"
    Write-Output "uh = RotateLeft(uh, 2) ^ vh;"
    Write-Output "vh = RotateLeft(vh, 14) ^ uh;"
    Write-Output "uh = RotateLeft(uh, 10) ^ vh;"
    Write-Output "vh = RotateLeft(vh, 1);"
    Write-Output "$u = ul | ((ulong)uh << 32);"
    Write-Output "$v = vl | ((ulong)vh << 32);"
}

function MIX_WORD($params)
{
    $u = $params[0]
    $v = $params[1]

    Write-Output "$v ^= $u;"
    Write-Output "$u = RotateLeft($u, 2) ^ $v;"
    Write-Output "$v = RotateLeft($v, 14) ^ $u;"
    Write-Output "$u = RotateLeft($u, 10) ^ $v;"
    Write-Output "$v = RotateLeft($v, 1);"
}

function P5
{
    Write-Output "ulong w0, w1, w2, w3, w4, w5, w6, w7;"

    Write-Output ""

    TWEAK5

    Write-Output ""

    Write-Output "w0 = v00 | ((ulong)v10 << 32);"
    Write-Output "w1 = v01 | ((ulong)v11 << 32);"
    Write-Output "w2 = v02 | ((ulong)v12 << 32);"
    Write-Output "w3 = v03 | ((ulong)v13 << 32);"
    Write-Output "w4 = v04 | ((ulong)v14 << 32);"
    Write-Output "w5 = v05 | ((ulong)v15 << 32);"
    Write-Output "w6 = v06 | ((ulong)v16 << 32);"
    Write-Output "w7 = v07 | ((ulong)v17 << 32);"

    Write-Output ""

    Write-Output "for (int r = 0; r < 8; r++)"
    Write-Output "{"
    Write-Output "ulong ltmp;"
    Write-Output "ulong ul, uh, vl, vh;"
    SUB_CRUMBW('w0', 'w1', 'w2', 'w3')
    SUB_CRUMBW('w5', 'w6', 'w7', 'w4')
    MIX_WORDW('w0', 'w4')
    MIX_WORDW('w1', 'w5')
    MIX_WORDW('w2', 'w6')
    MIX_WORDW('w3', 'w7')
    Write-Output "w0 ^= RCW010[r];"
    Write-Output "w4 ^= RCW014[r];"
    Write-Output "}"

    Write-Output ""

    Write-Output "v00 = (uint)w0;"
    Write-Output "v10 = (uint)(w0 >> 32);"
    Write-Output "v01 = (uint)w1;"
    Write-Output "v11 = (uint)(w1 >> 32);"
    Write-Output "v02 = (uint)w2;"
    Write-Output "v12 = (uint)(w2 >> 32);"
    Write-Output "v03 = (uint)w3;"
    Write-Output "v13 = (uint)(w3 >> 32);"
    Write-Output "v04 = (uint)w4;"
    Write-Output "v14 = (uint)(w4 >> 32);"
    Write-Output "v05 = (uint)w5;"
    Write-Output "v15 = (uint)(w5 >> 32);"
    Write-Output "v06 = (uint)w6;"
    Write-Output "v16 = (uint)(w6 >> 32);"
    Write-Output "v07 = (uint)w7;"
    Write-Output "v17 = (uint)(w7 >> 32);"

    Write-Output ""

    Write-Output "w0 = v20 | ((ulong)v30 << 32);"
    Write-Output "w1 = v21 | ((ulong)v31 << 32);"
    Write-Output "w2 = v22 | ((ulong)v32 << 32);"
    Write-Output "w3 = v23 | ((ulong)v33 << 32);"
    Write-Output "w4 = v24 | ((ulong)v34 << 32);"
    Write-Output "w5 = v25 | ((ulong)v35 << 32);"
    Write-Output "w6 = v26 | ((ulong)v36 << 32);"
    Write-Output "w7 = v27 | ((ulong)v37 << 32);"

    Write-Output ""

    Write-Output "for (int r = 0; r < 8; r++)"
    Write-Output "{"
    Write-Output "ulong ltmp;"
    Write-Output "ulong ul, uh, vl, vh;"
    SUB_CRUMBW('w0', 'w1', 'w2', 'w3')
    SUB_CRUMBW('w5', 'w6', 'w7', 'w4')
    MIX_WORDW('w0', 'w4')
    MIX_WORDW('w1', 'w5')
    MIX_WORDW('w2', 'w6')
    MIX_WORDW('w3', 'w7')
    Write-Output "w0 ^= RCW230[r];"
    Write-Output "w4 ^= RCW234[r];"
    Write-Output "}"

    Write-Output ""

    Write-Output "v20 = (uint)w0;"
    Write-Output "v30 = (uint)(w0 >> 32);"
    Write-Output "v21 = (uint)w1;"
    Write-Output "v31 = (uint)(w1 >> 32);"
    Write-Output "v22 = (uint)w2;"
    Write-Output "v32 = (uint)(w2 >> 32);"
    Write-Output "v23 = (uint)w3;"
    Write-Output "v33 = (uint)(w3 >> 32);"
    Write-Output "v24 = (uint)w4;"
    Write-Output "v34 = (uint)(w4 >> 32);"
    Write-Output "v25 = (uint)w5;"
    Write-Output "v35 = (uint)(w5 >> 32);"
    Write-Output "v26 = (uint)w6;"
    Write-Output "v36 = (uint)(w6 >> 32);"
    Write-Output "v27 = (uint)w7;"
    Write-Output "v37 = (uint)(w7 >> 32);"

    Write-Output ""

    Write-Output "for (int r = 0; r < 8; r++)"
    Write-Output "{"
    SUB_CRUMB('v40', 'v41', 'v42', 'v43')
    SUB_CRUMB('v45', 'v46', 'v47', 'v44')
    MIX_WORD('v40', 'v44')
    MIX_WORD('v41', 'v45')
    MIX_WORD('v42', 'v46')
    MIX_WORD('v43', 'v47')
    Write-Output "v40 ^= RC40[r];"
    Write-Output "v44 ^= RC44[r];"
    Write-Output "}"
}

function ProcessBlock
{
    for ($i = 0; $i -lt 5; $i++)
    {
        for ($j = 0; $j -lt 8; $j++)
        {
            Write-Output "uint v$i$j = h$i$j;"
        }
    }

    Write-Output ""

    Write-Output "#region MI5"
    Write-Output ""
    MI5
    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""

    Write-Output "#region P5"
    Write-Output ""
    P5
    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""

    for ($i = 0; $i -lt 5; $i++)
    {
        for ($j = 0; $j -lt 8; $j++)
        {
            Write-Output "h$i$j = v$i$j;"
        }
    }
}

Clear-Host

#State
#Reset
ProcessBlock
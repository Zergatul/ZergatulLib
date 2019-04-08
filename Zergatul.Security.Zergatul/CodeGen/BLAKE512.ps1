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

$Z = @{
    Z00 = '0'
    Z01 = '1'
    Z02 = '2'
    Z03 = '3'
    Z04 = '4'
    Z05 = '5'
    Z06 = '6'
    Z07 = '7'
    Z08 = '8'
    Z09 = '9'
    Z0A = 'A'
    Z0B = 'B'
    Z0C = 'C'
    Z0D = 'D'
    Z0E = 'E'
    Z0F = 'F'

    Z10 = 'E'
    Z11 = 'A'
    Z12 = '4'
    Z13 = '8'
    Z14 = '9'
    Z15 = 'F'
    Z16 = 'D'
    Z17 = '6'
    Z18 = '1'
    Z19 = 'C'
    Z1A = '0'
    Z1B = '2'
    Z1C = 'B'
    Z1D = '7'
    Z1E = '5'
    Z1F = '3'

    Z20 = 'B'
    Z21 = '8'
    Z22 = 'C'
    Z23 = '0'
    Z24 = '5'
    Z25 = '2'
    Z26 = 'F'
    Z27 = 'D'
    Z28 = 'A'
    Z29 = 'E'
    Z2A = '3'
    Z2B = '6'
    Z2C = '7'
    Z2D = '1'
    Z2E = '9'
    Z2F = '4'

    Z30 = '7'
    Z31 = '9'
    Z32 = '3'
    Z33 = '1'
    Z34 = 'D'
    Z35 = 'C'
    Z36 = 'B'
    Z37 = 'E'
    Z38 = '2'
    Z39 = '6'
    Z3A = '5'
    Z3B = 'A'
    Z3C = '4'
    Z3D = '0'
    Z3E = 'F'
    Z3F = '8'

    Z40 = '9'
    Z41 = '0'
    Z42 = '5'
    Z43 = '7'
    Z44 = '2'
    Z45 = '4'
    Z46 = 'A'
    Z47 = 'F'
    Z48 = 'E'
    Z49 = '1'
    Z4A = 'B'
    Z4B = 'C'
    Z4C = '6'
    Z4D = '8'
    Z4E = '3'
    Z4F = 'D'

    Z50 = '2'
    Z51 = 'C'
    Z52 = '6'
    Z53 = 'A'
    Z54 = '0'
    Z55 = 'B'
    Z56 = '8'
    Z57 = '3'
    Z58 = '4'
    Z59 = 'D'
    Z5A = '7'
    Z5B = '5'
    Z5C = 'F'
    Z5D = 'E'
    Z5E = '1'
    Z5F = '9'

    Z60 = 'C'
    Z61 = '5'
    Z62 = '1'
    Z63 = 'F'
    Z64 = 'E'
    Z65 = 'D'
    Z66 = '4'
    Z67 = 'A'
    Z68 = '0'
    Z69 = '7'
    Z6A = '6'
    Z6B = '3'
    Z6C = '9'
    Z6D = '2'
    Z6E = '8'
    Z6F = 'B'

    Z70 = 'D'
    Z71 = 'B'
    Z72 = '7'
    Z73 = 'E'
    Z74 = 'C'
    Z75 = '1'
    Z76 = '3'
    Z77 = '9'
    Z78 = '5'
    Z79 = '0'
    Z7A = 'F'
    Z7B = '4'
    Z7C = '8'
    Z7D = '6'
    Z7E = '2'
    Z7F = 'A'

    Z80 = '6'
    Z81 = 'F'
    Z82 = 'E'
    Z83 = '9'
    Z84 = 'B'
    Z85 = '3'
    Z86 = '0'
    Z87 = '8'
    Z88 = 'C'
    Z89 = '2'
    Z8A = 'D'
    Z8B = '7'
    Z8C = '1'
    Z8D = '4'
    Z8E = 'A'
    Z8F = '5'

    Z90 = 'A'
    Z91 = '2'
    Z92 = '8'
    Z93 = '4'
    Z94 = '7'
    Z95 = '6'
    Z96 = '1'
    Z97 = '5'
    Z98 = 'F'
    Z99 = 'B'
    Z9A = '9'
    Z9B = 'E'
    Z9C = '3'
    Z9D = 'C'
    Z9E = 'D'
    Z9F = '0'
}

$CB = @(
    '0x243F6A8885A308D3'
    '0x13198A2E03707344'
    '0xA4093822299F31D0'
    '0x082EFA98EC4E6C89'
    '0x452821E638D01377'
    '0xBE5466CF34E90C6C'
    '0xC0AC29B7C97C50DD'
    '0x3F84D5B5B5470917'
    '0x9216D5D98979FB1B'
    '0xD1310BA698DFB5AC'
    '0x2FFD72DBD01ADFB7'
    '0xB8E1AFED6A267E96'
    '0xBA7C9045F12C7F99'
    '0x24A19947B3916CF7'
    '0x0801F2E2858EFC16'
    '0x636920D871574E69'
)

function Mx($params)
{
    $r = $params[0]
    $i = $params[1]

    return ('m' + ($Z["Z" + $r + $i]).ToLower())
}

function CBx($params)
{
    $r = $params[0]
    $i = $params[1]

    $index = [System.Convert]::ToInt32($Z["Z" + $r + $i], 16)

    return $CB[$index]
}

function GB($params)
{
    $m0 = $params[0]
    $m1 = $params[1]
    $c0 = $params[2]
    $c1 = $params[3]
    $a = $params[4]
    $b = $params[5]
    $c = $params[6]
    $d = $params[7]

    Write-Output "$a += $b + ($m0 ^ $c1);"
    Write-Output "$d = RotateRight($d ^ $a, 32);"
    Write-Output "$c += $d;"
    Write-Output "$b = RotateRight($b ^ $c, 25);"
    Write-Output "$a += $b + ($m1 ^ $c0);"
    Write-Output "$d = RotateRight($d ^ $a, 16);"
    Write-Output "$c += $d;"
    Write-Output "$b = RotateRight($b ^ $c, 11);"
}

function ROUND_B($r)
{
    GB((Mx($r, '0')), (Mx($r, '1')), (CBx($r, '0')), (CBx($r, '1')), 'v0', 'v4', 'v8', 'vc')
    GB((Mx($r, '2')), (Mx($r, '3')), (CBx($r, '2')), (CBx($r, '3')), 'v1', 'v5', 'v9', 'vd')
    GB((Mx($r, '4')), (Mx($r, '5')), (CBx($r, '4')), (CBx($r, '5')), 'v2', 'v6', 'va', 've')
    GB((Mx($r, '6')), (Mx($r, '7')), (CBx($r, '6')), (CBx($r, '7')), 'v3', 'v7', 'vb', 'vf')
    GB((Mx($r, '8')), (Mx($r, '9')), (CBx($r, '8')), (CBx($r, '9')), 'v0', 'v5', 'va', 'vf')
    GB((Mx($r, 'a')), (Mx($r, 'b')), (CBx($r, 'a')), (CBx($r, 'b')), 'v1', 'v6', 'vb', 'vc')
    GB((Mx($r, 'c')), (Mx($r, 'd')), (CBx($r, 'c')), (CBx($r, 'd')), 'v2', 'v7', 'v8', 'vd')
    GB((Mx($r, 'e')), (Mx($r, 'f')), (CBx($r, 'e')), (CBx($r, 'f')), 'v3', 'v4', 'v9', 've')
}

function ProcessBlock
{
    for ($i = 0; $i -lt 16; $i++)
    {
        "ulong m$(hex($i)) = ToUInt64(buffer, 0x$(hex(($i * 8), 2)), ByteOrder.BigEndian);"
    }

    Write-Output ""

    for ($i = 0; $i -lt 8; $i++)
    {
        Write-Output "ulong v$i = h$i;"
    }
    Write-Output "ulong v8 = s0 ^ 0x243F6A8885A308D3;"
    Write-Output "ulong v9 = s1 ^ 0x13198A2E03707344;"
    Write-Output "ulong va = s2 ^ 0xA4093822299F31D0;"
    Write-Output "ulong vb = s3 ^ 0x082EFA98EC4E6C89;"
    Write-Output "ulong vc = lengthLo ^ 0x452821E638D01377;"
    Write-Output "ulong vd = lengthLo ^ 0xBE5466CF34E90C6C;"
    Write-Output "ulong ve = lengthHi ^ 0xC0AC29B7C97C50DD;"
    Write-Output "ulong vf = lengthHi ^ 0x3F84D5B5B5470917;"

    Write-Output ""
    Write-Output "#region Loop"

    Write-Output ""
    Write-Output "// Iteration 1"
    ROUND_B(0)

    Write-Output ""
    Write-Output "// Iteration 2"
    ROUND_B(1)

    Write-Output ""
    Write-Output "// Iteration 3"
    ROUND_B(2)

    Write-Output ""
    Write-Output "// Iteration 4"
    ROUND_B(3)

    Write-Output ""
    Write-Output "// Iteration 5"
    ROUND_B(4)

    Write-Output ""
    Write-Output "// Iteration 6"
    ROUND_B(5)

    Write-Output ""
    Write-Output "// Iteration 7"
    ROUND_B(6)

    Write-Output ""
    Write-Output "// Iteration 8"
    ROUND_B(7)

    Write-Output ""
    Write-Output "// Iteration 9"
    ROUND_B(8)

    Write-Output ""
    Write-Output "// Iteration 10"
    ROUND_B(9)

    Write-Output ""
    Write-Output "// Iteration 11"
    ROUND_B(0)

    Write-Output ""
    Write-Output "// Iteration 12"
    ROUND_B(1)

    Write-Output ""
    Write-Output "// Iteration 13"
    ROUND_B(2)

    Write-Output ""
    Write-Output "// Iteration 14"
    ROUND_B(3)

    Write-Output ""
    Write-Output "// Iteration 15"
    ROUND_B(4)

    Write-Output ""
    Write-Output "// Iteration 16"
    ROUND_B(5)

    Write-Output ""
    Write-Output "#endregion"
    Write-Output ""

    Write-Output "h0 ^= s0 ^ v0 ^ v8;"
    Write-Output "h1 ^= s1 ^ v1 ^ v9;"
    Write-Output "h2 ^= s2 ^ v2 ^ va;"
    Write-Output "h3 ^= s3 ^ v3 ^ vb;"
    Write-Output "h4 ^= s0 ^ v4 ^ vc;"
    Write-Output "h5 ^= s1 ^ v5 ^ vd;"
    Write-Output "h6 ^= s2 ^ v6 ^ ve;"
    Write-Output "h7 ^= s3 ^ v7 ^ vf;"
}

clear

ProcessBlock
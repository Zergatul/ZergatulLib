$RC = @(
	'0x0000000000000001', '0x0000000000008082',
	'0x800000000000808A', '0x8000000080008000',
	'0x000000000000808B', '0x0000000080000001',
	'0x8000000080008081', '0x8000000000008009',
	'0x000000000000008A', '0x0000000000000088',
	'0x0000000080008009', '0x000000008000000A',
	'0x000000008000808B', '0x800000000000008B',
	'0x8000000000008089', '0x8000000000008003',
	'0x8000000000008002', '0x8000000000000080',
	'0x000000000000800A', '0x800000008000000A',
	'0x8000000080008081', '0x8000000000008080',
	'0x0000000080000001', '0x8000000080008008')

$P = @(
    'a00,a01,a02,a03,a04,a10,a11,a12,a13,a14,a20,a21,a22,a23,a24,a30,a31,a32,a33,a34,a40,a41,a42,a43,a44'
    'a00,a30,a10,a40,a20,a11,a41,a21,a01,a31,a22,a02,a32,a12,a42,a33,a13,a43,a23,a03,a44,a24,a04,a34,a14'
    'a00,a33,a11,a44,a22,a41,a24,a02,a30,a13,a32,a10,a43,a21,a04,a23,a01,a34,a12,a40,a14,a42,a20,a03,a31'
    'a00,a23,a41,a14,a32,a24,a42,a10,a33,a01,a43,a11,a34,a02,a20,a12,a30,a03,a21,a44,a31,a04,a22,a40,a13'
    'a00,a12,a24,a31,a43,a42,a04,a11,a23,a30,a34,a41,a03,a10,a22,a21,a33,a40,a02,a14,a13,a20,a32,a44,a01'
    'a00,a21,a42,a13,a34,a04,a20,a41,a12,a33,a03,a24,a40,a11,a32,a02,a23,a44,a10,a31,a01,a22,a43,a14,a30'
    'a00,a02,a04,a01,a03,a20,a22,a24,a21,a23,a40,a42,a44,a41,a43,a10,a12,a14,a11,a13,a30,a32,a34,a31,a33'
    'a00,a10,a20,a30,a40,a22,a32,a42,a02,a12,a44,a04,a14,a24,a34,a11,a21,a31,a41,a01,a33,a43,a03,a13,a23'
    'a00,a11,a22,a33,a44,a32,a43,a04,a10,a21,a14,a20,a31,a42,a03,a41,a02,a13,a24,a30,a23,a34,a40,a01,a12'
    'a00,a41,a32,a23,a14,a43,a34,a20,a11,a02,a31,a22,a13,a04,a40,a24,a10,a01,a42,a33,a12,a03,a44,a30,a21'
    'a00,a24,a43,a12,a31,a34,a03,a22,a41,a10,a13,a32,a01,a20,a44,a42,a11,a30,a04,a23,a21,a40,a14,a33,a02'
    'a00,a42,a34,a21,a13,a03,a40,a32,a24,a11,a01,a43,a30,a22,a14,a04,a41,a33,a20,a12,a02,a44,a31,a23,a10'
    'a00,a04,a03,a02,a01,a40,a44,a43,a42,a41,a30,a34,a33,a32,a31,a20,a24,a23,a22,a21,a10,a14,a13,a12,a11'
    'a00,a20,a40,a10,a30,a44,a14,a34,a04,a24,a33,a03,a23,a43,a13,a22,a42,a12,a32,a02,a11,a31,a01,a21,a41'
    'a00,a22,a44,a11,a33,a14,a31,a03,a20,a42,a23,a40,a12,a34,a01,a32,a04,a21,a43,a10,a41,a13,a30,a02,a24'
    'a00,a32,a14,a41,a23,a31,a13,a40,a22,a04,a12,a44,a21,a03,a30,a43,a20,a02,a34,a11,a24,a01,a33,a10,a42'
    'a00,a43,a31,a24,a12,a13,a01,a44,a32,a20,a21,a14,a02,a40,a33,a34,a22,a10,a03,a41,a42,a30,a23,a11,a04'
    'a00,a34,a13,a42,a21,a01,a30,a14,a43,a22,a02,a31,a10,a44,a23,a03,a32,a11,a40,a24,a04,a33,a12,a41,a20'
    'a00,a03,a01,a04,a02,a30,a33,a31,a34,a32,a10,a13,a11,a14,a12,a40,a43,a41,a44,a42,a20,a23,a21,a24,a22'
    'a00,a40,a30,a20,a10,a33,a23,a13,a03,a43,a11,a01,a41,a31,a21,a44,a34,a24,a14,a04,a22,a12,a02,a42,a32'
    'a00,a44,a33,a22,a11,a23,a12,a01,a40,a34,a41,a30,a24,a13,a02,a14,a03,a42,a31,a20,a32,a21,a10,a04,a43'
    'a00,a14,a23,a32,a41,a12,a21,a30,a44,a03,a24,a33,a42,a01,a10,a31,a40,a04,a13,a22,a43,a02,a11,a20,a34'
    'a00,a31,a12,a43,a24,a21,a02,a33,a14,a40,a42,a23,a04,a30,a11,a13,a44,a20,a01,a32,a34,a10,a41,a22,a03'
    'a00,a13,a21,a34,a42,a02,a10,a23,a31,a44,a04,a12,a20,a33,a41,a01,a14,a22,a30,a43,a03,a11,a24,a32,a40'
)

function DefState
{
    for ($i = 0; $i -lt 5; $i++)
    {
        $line = "ulong "
        for ($j = 0; $j -lt 5; $j++)
        {
            $line += "s$i$j, "
        }
        Write-Output ($line.Substring(0, $line.Length - 2) + ";")
    }
}

function ThElt($params)
{
    $t  = $params[0]
    $c0 = $params[1]
    $c1 = $params[2]
    $c2 = $params[3]
    $c3 = $params[4]
    $c4 = $params[5]
    $d0 = $params[6]
    $d1 = $params[7]
    $d2 = $params[8]
    $d3 = $params[9]
    $d4 = $params[10]

    Write-Output "tt0 = $d0 ^ $d1;"
    Write-Output "tt1 = $d2 ^ $d3;"
    Write-Output "tt0 ^= $d4;"
    Write-Output "tt0 ^= tt1;"
    Write-Output "tt0 = RotateLeft(tt0, 1);"
    Write-Output "tt2 = $c0 ^ $c1;"
    Write-Output "tt3 = $c2 ^ $c3;"
    Write-Output "tt0 ^= $c4;"
    Write-Output "tt2 ^= tt3;"
    Write-Output "$t = tt0 ^ tt2;"
}

function Theta($params)
{
    $params = $params.Split(',')
    $b00 = $params[0]
    $b01 = $params[1]
    $b02 = $params[2]
    $b03 = $params[3]
    $b04 = $params[4]
    $b10 = $params[5]
    $b11 = $params[6]
    $b12 = $params[7]
    $b13 = $params[8]
    $b14 = $params[9]
    $b20 = $params[10]
    $b21 = $params[11]
    $b22 = $params[12]
    $b23 = $params[13]
    $b24 = $params[14]
    $b30 = $params[15]
    $b31 = $params[16]
    $b32 = $params[17]
    $b33 = $params[18]
    $b34 = $params[19]
    $b40 = $params[20]
    $b41 = $params[21]
    $b42 = $params[22]
    $b43 = $params[23]
    $b44 = $params[24]

    ThElt('t0', $b40, $b41, $b42, $b43, $b44, $b10, $b11, $b12, $b13, $b14)
    ThElt('t1', $b00, $b01, $b02, $b03, $b04, $b20, $b21, $b22, $b23, $b24)
    ThElt('t2', $b10, $b11, $b12, $b13, $b14, $b30, $b31, $b32, $b33, $b34)
    ThElt('t3', $b20, $b21, $b22, $b23, $b24, $b40, $b41, $b42, $b43, $b44)
    ThElt('t4', $b30, $b31, $b32, $b33, $b34, $b00, $b01, $b02, $b03, $b04)

    Write-Output "$b00 ^= t0;"
    Write-Output "$b01 ^= t0;"
    Write-Output "$b02 ^= t0;"
    Write-Output "$b03 ^= t0;"
    Write-Output "$b04 ^= t0;"
    Write-Output "$b10 ^= t1;"
    Write-Output "$b11 ^= t1;"
    Write-Output "$b12 ^= t1;"
    Write-Output "$b13 ^= t1;"
    Write-Output "$b14 ^= t1;"
    Write-Output "$b20 ^= t2;"
    Write-Output "$b21 ^= t2;"
    Write-Output "$b22 ^= t2;"
    Write-Output "$b23 ^= t2;"
    Write-Output "$b24 ^= t2;"
    Write-Output "$b30 ^= t3;"
    Write-Output "$b31 ^= t3;"
    Write-Output "$b32 ^= t3;"
    Write-Output "$b33 ^= t3;"
    Write-Output "$b34 ^= t3;"
    Write-Output "$b40 ^= t4;"
    Write-Output "$b41 ^= t4;"
    Write-Output "$b42 ^= t4;"
    Write-Output "$b43 ^= t4;"
    Write-Output "$b44 ^= t4;"
}

function Pho($params)
{
    $params = $params.Split(',')
    $b00 = $params[0]
    $b01 = $params[1]
    $b02 = $params[2]
    $b03 = $params[3]
    $b04 = $params[4]
    $b10 = $params[5]
    $b11 = $params[6]
    $b12 = $params[7]
    $b13 = $params[8]
    $b14 = $params[9]
    $b20 = $params[10]
    $b21 = $params[11]
    $b22 = $params[12]
    $b23 = $params[13]
    $b24 = $params[14]
    $b30 = $params[15]
    $b31 = $params[16]
    $b32 = $params[17]
    $b33 = $params[18]
    $b34 = $params[19]
    $b40 = $params[20]
    $b41 = $params[21]
    $b42 = $params[22]
    $b43 = $params[23]
    $b44 = $params[24]

	Write-Output "$b01 = RotateLeft($b01, 36);"
	Write-Output "$b02 = RotateLeft($b02,  3);"
	Write-Output "$b03 = RotateLeft($b03, 41);"
	Write-Output "$b04 = RotateLeft($b04, 18);"
	Write-Output "$b10 = RotateLeft($b10,  1);"
	Write-Output "$b11 = RotateLeft($b11, 44);"
	Write-Output "$b12 = RotateLeft($b12, 10);"
	Write-Output "$b13 = RotateLeft($b13, 45);"
	Write-Output "$b14 = RotateLeft($b14,  2);"
	Write-Output "$b20 = RotateLeft($b20, 62);"
	Write-Output "$b21 = RotateLeft($b21,  6);"
	Write-Output "$b22 = RotateLeft($b22, 43);"
	Write-Output "$b23 = RotateLeft($b23, 15);"
	Write-Output "$b24 = RotateLeft($b24, 61);"
	Write-Output "$b30 = RotateLeft($b30, 28);"
	Write-Output "$b31 = RotateLeft($b31, 55);"
	Write-Output "$b32 = RotateLeft($b32, 25);"
	Write-Output "$b33 = RotateLeft($b33, 21);"
	Write-Output "$b34 = RotateLeft($b34, 56);"
	Write-Output "$b40 = RotateLeft($b40, 27);"
	Write-Output "$b41 = RotateLeft($b41, 20);"
	Write-Output "$b42 = RotateLeft($b42, 39);"
	Write-Output "$b43 = RotateLeft($b43,  8);"
	Write-Output "$b44 = RotateLeft($b44, 14);"
}

function Khi_XO($params)
{
    $d = $params[0]
    $a = $params[1]
    $b = $params[2]
    $c = $params[3]

    Write-Output "$d = $a ^ ($b | $c);"
}

function Khi_XA($params)
{
    $d = $params[0]
    $a = $params[1]
    $b = $params[2]
    $c = $params[3]

    Write-Output "$d = $a ^ ($b & $c);"
}

function Khi($params)
{
    $params = $params.Split(',')
    $b00 = $params[0]
    $b01 = $params[1]
    $b02 = $params[2]
    $b03 = $params[3]
    $b04 = $params[4]
    $b10 = $params[5]
    $b11 = $params[6]
    $b12 = $params[7]
    $b13 = $params[8]
    $b14 = $params[9]
    $b20 = $params[10]
    $b21 = $params[11]
    $b22 = $params[12]
    $b23 = $params[13]
    $b24 = $params[14]
    $b30 = $params[15]
    $b31 = $params[16]
    $b32 = $params[17]
    $b33 = $params[18]
    $b34 = $params[19]
    $b40 = $params[20]
    $b41 = $params[21]
    $b42 = $params[22]
    $b43 = $params[23]
    $b44 = $params[24]

    $bnn = 'bnn'

    Write-Output "$bnn = ~$b20;"

    Khi_XO('t0', $b00, $b10, $b20)
    Khi_XO('t1', $b10, $bnn, $b30)
    Khi_XA('t2', $b20, $b30, $b40)
    Khi_XO('t3', $b30, $b40, $b00)
    Khi_XA('t4', $b40, $b00, $b10)

    Write-Output "$b00 = t0;"
    Write-Output "$b10 = t1;"
    Write-Output "$b20 = t2;"
    Write-Output "$b30 = t3;"
    Write-Output "$b40 = t4;"
    Write-Output "$bnn = ~$b41;"

    Khi_XO('t0', $b01, $b11, $b21)
    Khi_XA('t1', $b11, $b21, $b31)
    Khi_XO('t2', $b21, $b31, $bnn)
    Khi_XO('t3', $b31, $b41, $b01)
    Khi_XA('t4', $b41, $b01, $b11)

    Write-Output "$b01 = t0;"
    Write-Output "$b11 = t1;"
    Write-Output "$b21 = t2;"
    Write-Output "$b31 = t3;"
    Write-Output "$b41 = t4;"
    Write-Output "$bnn = ~$b32;"

    Khi_XO('t0', $b02, $b12, $b22)
    Khi_XA('t1', $b12, $b22, $b32)
    Khi_XA('t2', $b22, $bnn, $b42)
    Khi_XO('t3', $bnn, $b42, $b02)
    Khi_XA('t4', $b42, $b02, $b12)

    Write-Output "$b02 = t0;"
    Write-Output "$b12 = t1;"
    Write-Output "$b22 = t2;"
    Write-Output "$b32 = t3;"
    Write-Output "$b42 = t4;"
    Write-Output "$bnn = ~$b33;"

    Khi_XA('t0', $b03, $b13, $b23)
    Khi_XO('t1', $b13, $b23, $b33)
    Khi_XO('t2', $b23, $bnn, $b43)
    Khi_XA('t3', $bnn, $b43, $b03)
    Khi_XO('t4', $b43, $b03, $b13)

    Write-Output "$b03 = t0;"
    Write-Output "$b13 = t1;"
    Write-Output "$b23 = t2;"
    Write-Output "$b33 = t3;"
    Write-Output "$b43 = t4;"
    Write-Output "$bnn = ~$b14;"

    Khi_XA('t0', $b04, $bnn, $b24)
    Khi_XO('t1', $bnn, $b24, $b34)
    Khi_XA('t2', $b24, $b34, $b44)
    Khi_XO('t3', $b34, $b44, $b04)
    Khi_XA('t4', $b44, $b04, $b14)

    Write-Output "$b04 = t0;"
    Write-Output "$b14 = t1;"
    Write-Output "$b24 = t2;"
    Write-Output "$b34 = t3;"
    Write-Output "$b44 = t4;"
}

function KFElt($params)
{
    $r = $params[0]
    $s = $params[1]
    $k = $params[2]

    Theta($P[$r])
    Pho($P[$r])
    Khi($P[$s])
    Write-Output "a00 ^= $k;"
}

function Core
{
    for ($i = 0; $i -lt 5; $i++)
    {
        for ($j = 0; $j -lt 5; $j++)
        {
            Write-Output "ulong a$j$i = s$j$i;"
        }
    }

    Write-Output ""

    Write-Output "a00 ^= ToUInt64(buffer, 0x00, ByteOrder.LittleEndian);"
    Write-Output "a10 ^= ToUInt64(buffer, 0x08, ByteOrder.LittleEndian);"
    Write-Output "a20 ^= ToUInt64(buffer, 0x10, ByteOrder.LittleEndian);"
    Write-Output "a30 ^= ToUInt64(buffer, 0x18, ByteOrder.LittleEndian);"
    Write-Output "a40 ^= ToUInt64(buffer, 0x20, ByteOrder.LittleEndian);"
    Write-Output "a01 ^= ToUInt64(buffer, 0x28, ByteOrder.LittleEndian);"
    Write-Output "a11 ^= ToUInt64(buffer, 0x30, ByteOrder.LittleEndian);"
    Write-Output "a21 ^= ToUInt64(buffer, 0x38, ByteOrder.LittleEndian);"
    Write-Output "a31 ^= ToUInt64(buffer, 0x40, ByteOrder.LittleEndian);"
    Write-Output "if (limit > 72)"
    Write-Output "{"
    Write-Output "a41 ^= ToUInt64(buffer, 0x48, ByteOrder.LittleEndian);"
    Write-Output "a02 ^= ToUInt64(buffer, 0x50, ByteOrder.LittleEndian);"
    Write-Output "a12 ^= ToUInt64(buffer, 0x58, ByteOrder.LittleEndian);"
    Write-Output "a22 ^= ToUInt64(buffer, 0x60, ByteOrder.LittleEndian);"
    Write-Output "if (limit > 104)"
    Write-Output "{"
    Write-Output "a32 ^= ToUInt64(buffer, 0x70, ByteOrder.LittleEndian);"
    Write-Output "a42 ^= ToUInt64(buffer, 0x78, ByteOrder.LittleEndian);"
    Write-Output "a03 ^= ToUInt64(buffer, 0x80, ByteOrder.LittleEndian);"
    Write-Output "a13 ^= ToUInt64(buffer, 0x88, ByteOrder.LittleEndian);"
    Write-Output "if (limit > 136)"
    Write-Output "{"
    Write-Output "a23 ^= ToUInt64(buffer, 0x90, ByteOrder.LittleEndian);"
    Write-Output "}"
    Write-Output "}"
    Write-Output "}"

    Write-Output ""

    Write-Output "#region Loop"
    Write-Output ""

    Write-Output "ulong t0, t1, t2, t3, t4;"
    Write-Output "ulong tt0, tt1, tt2, tt3;"
    Write-Output "ulong bnn;"
    Write-Output ""

    for ([int]$i = 0; $i -lt 24; $i++)
    {
        Write-Output "// Iteration $($i + 1)"
        KFElt($i, (($i + 1) % 24), $RC[$i])
        Write-Output ""
    }

    Write-Output "#endregion"

    Write-Output ""

    for ($i = 0; $i -lt 5; $i++)
    {
        for ($j = 0; $j -lt 5; $j++)
        {
            Write-Output "s$i$j = a$i$j;"
        }
    }
}

clear
Core
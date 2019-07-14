$s00 = 'm0'
$s01 = 'm1'
$s02 = 'c0'
$s03 = 'c1'
$s04 = 'm2'
$s05 = 'm3'
$s06 = 'c2'
$s07 = 'c3'
$s08 = 'c4'
$s09 = 'c5'
$s0a = 'm4'
$s0b = 'm5'
$s0c = 'c6'
$s0d = 'c7'
$s0e = 'm6'
$s0f = 'm7'
$s10 = 'm8'
$s11 = 'm9'
$s12 = 'c8'
$s13 = 'c9'
$s14 = 'ma'
$s15 = 'mb'
$s16 = 'ca'
$s17 = 'cb'
$s18 = 'cc'
$s19 = 'cd'
$s1a = 'mc'
$s1b = 'md'
$s1c = 'ce'
$s1d = 'cf'
$s1e = 'me'
$s1f = 'mf'

$s0 = 'm0'
$s1 = 'm1'
$s2 = 'c0'
$s3 = 'c1'
$s4 = 'c2'
$s5 = 'c3'
$s6 = 'm2'
$s7 = 'm3'
$s8 = 'm4'
$s9 = 'm5'
$sA = 'c4'
$sB = 'c5'
$sC = 'c6'
$sD = 'c7'
$sE = 'm6'
$sF = 'm7'

$alpha_n =
@(
'0xFF00F0F0', '0xCCCCAAAA', '0xF0F0CCCC',
'0xFF00AAAA', '0xCCCCAAAA', '0xF0F0FF00',
'0xAAAACCCC', '0xF0F0FF00', '0xF0F0CCCC',
'0xAAAAFF00', '0xCCCCFF00', '0xAAAAF0F0',
'0xAAAAF0F0', '0xFF00CCCC', '0xCCCCF0F0',
'0xFF00AAAA', '0xCCCCAAAA', '0xFF00F0F0',
'0xFF00AAAA', '0xF0F0CCCC', '0xF0F0FF00',
'0xCCCCAAAA', '0xF0F0FF00', '0xAAAACCCC',
'0xAAAAFF00', '0xF0F0CCCC', '0xAAAAF0F0',
'0xCCCCFF00', '0xFF00CCCC', '0xAAAAF0F0',
'0xFF00AAAA', '0xCCCCF0F0'
)

$alpha_f =
@(
'0xCAF9639C', '0x0FF0F9C0', '0x639C0FF0',
'0xCAF9F9C0', '0x0FF0F9C0', '0x639CCAF9',
'0xF9C00FF0', '0x639CCAF9', '0x639C0FF0',
'0xF9C0CAF9', '0x0FF0CAF9', '0xF9C0639C',
'0xF9C0639C', '0xCAF90FF0', '0x0FF0639C',
'0xCAF9F9C0', '0x0FF0F9C0', '0xCAF9639C',
'0xCAF9F9C0', '0x639C0FF0', '0x639CCAF9',
'0x0FF0F9C0', '0x639CCAF9', '0xF9C00FF0',
'0xF9C0CAF9', '0x639C0FF0', '0xF9C0639C',
'0x0FF0CAF9', '0xCAF90FF0', '0xF9C0639C',
'0xCAF9F9C0', '0x0FF0639C'
)

function SBOX($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]

	"t = $a;"
	"$a &= $c;"
	"$a ^= $d;"
	"$c ^= $b;"
	"$c ^= $a;"
	"$d |= t;"
	"$d ^= $b;"
	"t ^= $c;"
	"$b = $d;"
	"$d |= t;"
	"$d ^= $a;"
	"$a &= $b;"
	"t ^= $a;"
	"$b ^= $d;"
	"$b ^= t;"
	"$a = $c;"
	"$c = $b;"
	"$b = $d;"
	"$d = ~t;"
}

function L($params)
{
    $a = $params[0]
    $b = $params[1]
    $c = $params[2]
    $d = $params[3]

    "$a = RotateLeft($a, 13);"
	"$c = RotateLeft($c, 3);"
	"$b ^= $a ^ $c;"
	"$d ^= $c ^ ($a << 3);"
	"$b = RotateLeft($b, 1);"
	"$d = RotateLeft($d, 7);"
	"$a ^= $b ^ $d;"
	"$c ^= $d ^ ($b << 7);"
	"$a = RotateLeft($a, 5);"
	"$c = RotateLeft($c, 22);"
}

function ROUND_BIG($params)
{
    $rc = $params[0]
    $alpha = $params[1]

	"$s00 ^= $($alpha[0x00]);"
	"$s01 ^= $($alpha[0x01]) ^ $rc;"
	"$s02 ^= $($alpha[0x02]);"
	"$s03 ^= $($alpha[0x03]);"
	"$s04 ^= $($alpha[0x04]);"
	"$s05 ^= $($alpha[0x05]);"
	"$s06 ^= $($alpha[0x06]);"
	"$s07 ^= $($alpha[0x07]);"
	"$s08 ^= $($alpha[0x08]);"
	"$s09 ^= $($alpha[0x09]);"
	"$s0a ^= $($alpha[0x0A]);"
	"$s0b ^= $($alpha[0x0B]);"
	"$s0c ^= $($alpha[0x0C]);"
	"$s0d ^= $($alpha[0x0D]);"
	"$s0e ^= $($alpha[0x0E]);"
	"$s0f ^= $($alpha[0x0F]);"
	"$s10 ^= $($alpha[0x10]);"
	"$s11 ^= $($alpha[0x11]);"
	"$s12 ^= $($alpha[0x12]);"
	"$s13 ^= $($alpha[0x13]);"
	"$s14 ^= $($alpha[0x14]);"
	"$s15 ^= $($alpha[0x15]);"
	"$s16 ^= $($alpha[0x16]);"
	"$s17 ^= $($alpha[0x17]);"
	"$s18 ^= $($alpha[0x18]);"
	"$s19 ^= $($alpha[0x19]);"
	"$s1a ^= $($alpha[0x1A]);"
	"$s1b ^= $($alpha[0x1B]);"
	"$s1c ^= $($alpha[0x1C]);"
	"$s1d ^= $($alpha[0x1D]);"
	"$s1e ^= $($alpha[0x1E]);"
	"$s1f ^= $($alpha[0x1F]);"
    SBOX($s00, $s08, $s10, $s18)
    SBOX($s01, $s09, $s11, $s19)
    SBOX($s02, $s0a, $s12, $s1a)
    SBOX($s03, $s0b, $s13, $s1b)
    SBOX($s04, $s0c, $s14, $s1c)
    SBOX($s05, $s0d, $s15, $s1d)
    SBOX($s06, $s0e, $s16, $s1e)
    SBOX($s07, $s0f, $s17, $s1f)
    L($s00, $s09, $s12, $s1b)
    L($s01, $s0a, $s13, $s1c)
    L($s02, $s0b, $s14, $s1d)
    L($s03, $s0c, $s15, $s1e)
    L($s04, $s0d, $s16, $s1f)
    L($s05, $s0e, $s17, $s18)
    L($s06, $s0f, $s10, $s19)
    L($s07, $s08, $s11, $s1a)
    L($s00, $s02, $s05, $s07)
    L($s10, $s13, $s15, $s16)
    L($s09, $s0b, $s0c, $s0e)
    L($s19, $s1a, $s1c, $s1f)
}

function ROUND_SMALL($params)
{
    $rc = $params[0]
    $alpha = $params[1]

    "$s0 ^= $($alpha[0x00]);"
	"$s1 ^= $($alpha[0x01]) ^ $rc;"
	"$s2 ^= $($alpha[0x02]);"
	"$s3 ^= $($alpha[0x03]);"
	"$s4 ^= $($alpha[0x08]);"
	"$s5 ^= $($alpha[0x09]);"
	"$s6 ^= $($alpha[0x0A]);"
	"$s7 ^= $($alpha[0x0B]);"
	"$s8 ^= $($alpha[0x10]);"
	"$s9 ^= $($alpha[0x11]);"
	"$sA ^= $($alpha[0x12]);"
	"$sB ^= $($alpha[0x13]);"
	"$sC ^= $($alpha[0x18]);"
	"$sD ^= $($alpha[0x19]);"
	"$sE ^= $($alpha[0x1A]);"
	"$sF ^= $($alpha[0x1B]);"
	SBOX($s0, $s4, $s8, $sC);
	SBOX($s1, $s5, $s9, $sD);
	SBOX($s2, $s6, $sA, $sE);
	SBOX($s3, $s7, $sB, $sF);
	L($s0, $s5, $sA, $sF);
	L($s1, $s6, $sB, $sC);
	L($s2, $s7, $s8, $sD);
	L($s3, $s4, $s9, $sE);
}

function P_SMALL
{
    '#region P small'
    ''
    'uint t;'
    ''
    ROUND_SMALL(0, $alpha_n)
    ROUND_SMALL(1, $alpha_n)
    ROUND_SMALL(2, $alpha_n)
    ''
    '#endregion'
}

function P_BIG
{
    '#region P big'
    ''
    'uint t;'
    ''
    ROUND_BIG(0, $alpha_n)
    ROUND_BIG(1, $alpha_n)
    ROUND_BIG(2, $alpha_n)
    ROUND_BIG(3, $alpha_n)
    ROUND_BIG(4, $alpha_n)
    ROUND_BIG(5, $alpha_n)
    ''
    '#endregion'
}

function PF_SMALL
{
    '#region PF big'
    ''
    'uint t;'
    ''
    ROUND_SMALL(0, $alpha_f)
    ROUND_SMALL(1, $alpha_f)
    ROUND_SMALL(2, $alpha_f)
    ROUND_SMALL(3, $alpha_f)
    ROUND_SMALL(4, $alpha_f)
    ROUND_SMALL(5, $alpha_f)
    ''
    '#endregion'
}

function PF_BIG
{
    '#region PF small'
    ''
    'uint t;'
    ''
    ROUND_BIG(0, $alpha_f)
    ROUND_BIG(1, $alpha_f)
    ROUND_BIG(2, $alpha_f)
    ROUND_BIG(3, $alpha_f)
    ROUND_BIG(4, $alpha_f)
    ROUND_BIG(5, $alpha_f)
    ROUND_BIG(6, $alpha_f)
    ROUND_BIG(7, $alpha_f)
    ROUND_BIG(8, $alpha_f)
    ROUND_BIG(9, $alpha_f)
    ROUND_BIG(10, $alpha_f)
    ROUND_BIG(11, $alpha_f)
    ''
    '#endregion'
}

function T_SMALL
{
    '#region T small'
    ''
    "c7 = h7 ^= $sB;"
    "c6 = h6 ^= $sa;"
    "c5 = h5 ^= $s9;"
    "c4 = h4 ^= $s8;"
    "c3 = h3 ^= $s3;"
    "c2 = h2 ^= $s2;"
    "c1 = h1 ^= $s1;"
    "c0 = h0 ^= $s0;"
    ''
    '#endregion'
}

function T_BIG
{
    '#region T big'
    ''
    "cf = hf ^= $s17;"
    "ce = he ^= $s16;"
    "cd = hd ^= $s15;"
    "cc = hc ^= $s14;"
    "cb = hb ^= $s13;"
    "ca = ha ^= $s12;"
    "c9 = h9 ^= $s11;"
    "c8 = h8 ^= $s10;"
    "c7 = h7 ^= $s07;"
    "c6 = h6 ^= $s06;"
    "c5 = h5 ^= $s05;"
    "c4 = h4 ^= $s04;"
    "c3 = h3 ^= $s03;"
    "c2 = h2 ^= $s02;"
    "c1 = h1 ^= $s01;"
    "c0 = h0 ^= $s00;"
    ''
    '#endregion'
}

Clear-Host

#P_SMALL
PF_SMALL
#T_SMALL

#P_big
#PF_big
#T_BIG
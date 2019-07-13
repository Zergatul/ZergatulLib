#region Constants

$M9_0_0 = 0
$M9_0_1 = 1
$M9_0_2 = 2
$M9_0_3 = 3
$M9_0_4 = 4
$M9_0_5 = 5
$M9_0_6 = 6
$M9_0_7 = 7
$M9_1_0 = 1
$M9_1_1 = 2
$M9_1_2 = 3
$M9_1_3 = 4
$M9_1_4 = 5
$M9_1_5 = 6
$M9_1_6 = 7
$M9_1_7 = 8
$M9_2_0 = 2
$M9_2_1 = 3
$M9_2_2 = 4
$M9_2_3 = 5
$M9_2_4 = 6
$M9_2_5 = 7
$M9_2_6 = 8
$M9_2_7 = 0
$M9_3_0 = 3
$M9_3_1 = 4
$M9_3_2 = 5
$M9_3_3 = 6
$M9_3_4 = 7
$M9_3_5 = 8
$M9_3_6 = 0
$M9_3_7 = 1
$M9_4_0 = 4
$M9_4_1 = 5
$M9_4_2 = 6
$M9_4_3 = 7
$M9_4_4 = 8
$M9_4_5 = 0
$M9_4_6 = 1
$M9_4_7 = 2
$M9_5_0 = 5
$M9_5_1 = 6
$M9_5_2 = 7
$M9_5_3 = 8
$M9_5_4 = 0
$M9_5_5 = 1
$M9_5_6 = 2
$M9_5_7 = 3
$M9_6_0 = 6
$M9_6_1 = 7
$M9_6_2 = 8
$M9_6_3 = 0
$M9_6_4 = 1
$M9_6_5 = 2
$M9_6_6 = 3
$M9_6_7 = 4
$M9_7_0 = 7
$M9_7_1 = 8
$M9_7_2 = 0
$M9_7_3 = 1
$M9_7_4 = 2
$M9_7_5 = 3
$M9_7_6 = 4
$M9_7_7 = 5
$M9_8_0 = 8
$M9_8_1 = 0
$M9_8_2 = 1
$M9_8_3 = 2
$M9_8_4 = 3
$M9_8_5 = 4
$M9_8_6 = 5
$M9_8_7 = 6
$M9_9_0 = 0
$M9_9_1 = 1
$M9_9_2 = 2
$M9_9_3 = 3
$M9_9_4 = 4
$M9_9_5 = 5
$M9_9_6 = 6
$M9_9_7 = 7
$M9_10_0 = 1
$M9_10_1 = 2
$M9_10_2 = 3
$M9_10_3 = 4
$M9_10_4 = 5
$M9_10_5 = 6
$M9_10_6 = 7
$M9_10_7 = 8
$M9_11_0 = 2
$M9_11_1 = 3
$M9_11_2 = 4
$M9_11_3 = 5
$M9_11_4 = 6
$M9_11_5 = 7
$M9_11_6 = 8
$M9_11_7 = 0
$M9_12_0 = 3
$M9_12_1 = 4
$M9_12_2 = 5
$M9_12_3 = 6
$M9_12_4 = 7
$M9_12_5 = 8
$M9_12_6 = 0
$M9_12_7 = 1
$M9_13_0 = 4
$M9_13_1 = 5
$M9_13_2 = 6
$M9_13_3 = 7
$M9_13_4 = 8
$M9_13_5 = 0
$M9_13_6 = 1
$M9_13_7 = 2
$M9_14_0 = 5
$M9_14_1 = 6
$M9_14_2 = 7
$M9_14_3 = 8
$M9_14_4 = 0
$M9_14_5 = 1
$M9_14_6 = 2
$M9_14_7 = 3
$M9_15_0 = 6
$M9_15_1 = 7
$M9_15_2 = 8
$M9_15_3 = 0
$M9_15_4 = 1
$M9_15_5 = 2
$M9_15_6 = 3
$M9_15_7 = 4
$M9_16_0 = 7
$M9_16_1 = 8
$M9_16_2 = 0
$M9_16_3 = 1
$M9_16_4 = 2
$M9_16_5 = 3
$M9_16_6 = 4
$M9_16_7 = 5
$M9_17_0 = 8
$M9_17_1 = 0
$M9_17_2 = 1
$M9_17_3 = 2
$M9_17_4 = 3
$M9_17_5 = 4
$M9_17_6 = 5
$M9_17_7 = 6
$M9_18_0 = 0
$M9_18_1 = 1
$M9_18_2 = 2
$M9_18_3 = 3
$M9_18_4 = 4
$M9_18_5 = 5
$M9_18_6 = 6
$M9_18_7 = 7
$M3_0_0 = 0
$M3_0_1 = 1
$M3_1_0 = 1
$M3_1_1 = 2
$M3_2_0 = 2
$M3_2_1 = 0
$M3_3_0 = 0
$M3_3_1 = 1
$M3_4_0 = 1
$M3_4_1 = 2
$M3_5_0 = 2
$M3_5_1 = 0
$M3_6_0 = 0
$M3_6_1 = 1
$M3_7_0 = 1
$M3_7_1 = 2
$M3_8_0 = 2
$M3_8_1 = 0
$M3_9_0 = 0
$M3_9_1 = 1
$M3_10_0 = 1
$M3_10_1 = 2
$M3_11_0 = 2
$M3_11_1 = 0
$M3_12_0 = 0
$M3_12_1 = 1
$M3_13_0 = 1
$M3_13_1 = 2
$M3_14_0 = 2
$M3_14_1 = 0
$M3_15_0 = 0
$M3_15_1 = 1
$M3_16_0 = 1
$M3_16_1 = 2
$M3_17_0 = 2
$M3_17_1 = 0
$M3_18_0 = 0
$M3_18_1 = 1

#endregion

function TFBIG_KINIT($params)
{
   $k0 = $params[0]
   $k1 = $params[1]
   $k2 = $params[2]
   $k3 = $params[3]
   $k4 = $params[4]
   $k5 = $params[5]
   $k6 = $params[6]
   $k7 = $params[7]
   $k8 = $params[8]
   $t0 = $params[9]
   $t1 = $params[10]
   $t2 = $params[11]

   "$k8 = $k0 ^ $k1 ^ $k2 ^ $k3 ^ $k4 ^ $k5 ^ $k6 ^ $k7 ^ 0x1BD11BDAA9FC1A22;"
   "$t2 = $t0 ^ $t1;"
}

function SKBI($params)
{
    $k = $params[0]
    $s = $params[1]
    $i = $params[2]

    "$($k)$(Get-Variable "M9_$($s)_$i" -ValueOnly)"
}

function SKBT($params)
{
    $t = $params[0]
    $s = $params[1]
    $v = $params[2]

    "$($t)$(Get-Variable "M3_$($s)_$v" -ValueOnly)"
}

function TFBIG_ADDKEY($params)
{
    $w0 = $params[0]
    $w1 = $params[1]
    $w2 = $params[2]
    $w3 = $params[3]
    $w4 = $params[4]
    $w5 = $params[5]
    $w6 = $params[6]
    $w7 = $params[7]
    $k  = $params[8]
    $t  = $params[9]
    $s  = $params[10]

    "$w0 += $(SKBI($k, $s, 0));"
    "$w1 += $(SKBI($k, $s, 1));"
    "$w2 += $(SKBI($k, $s, 2));"
    "$w3 += $(SKBI($k, $s, 3));"
    "$w4 += $(SKBI($k, $s, 4));"
    "$w5 += $(SKBI($k, $s, 5)) + $(SKBT($t, $s, 0));"
    "$w6 += $(SKBI($k, $s, 6)) + $(SKBT($t, $s, 1));"
    "$w7 += $(SKBI($k, $s, 7)) + $s;"
}

function TFBIG_MIX($params)
{
    $x0 = $params[0]
    $x1 = $params[1]
    $rc = $params[2]

    "$x0 += $x1;"
    "$x1 = RotateLeft($x1, $rc) ^ $x0;"
}

function TFBIG_MIX8($params)
{
    $w0  = $params[0]
    $w1  = $params[1]
    $w2  = $params[2]
    $w3  = $params[3]
    $w4  = $params[4]
    $w5  = $params[5]
    $w6  = $params[6]
    $w7  = $params[7]
    $rc0 = $params[8]
    $rc1 = $params[9]
    $rc2 = $params[10]
    $rc3 = $params[11]

    TFBIG_MIX($w0, $w1, $rc0)
    TFBIG_MIX($w2, $w3, $rc1)
    TFBIG_MIX($w4, $w5, $rc2)
    TFBIG_MIX($w6, $w7, $rc3)
}

function TFBIG_4e($s)
{
    "#region TFBIG_4e($s)"
    TFBIG_ADDKEY('p0', 'p1', 'p2', 'p3', 'p4', 'p5', 'p6', 'p7', 'h', 't', $s)
	TFBIG_MIX8  ('p0', 'p1', 'p2', 'p3', 'p4', 'p5', 'p6', 'p7',  46,  36, 19, 37)
	TFBIG_MIX8  ('p2', 'p1', 'p4', 'p7', 'p6', 'p5', 'p0', 'p3',  33,  27, 14, 42)
	TFBIG_MIX8  ('p4', 'p1', 'p6', 'p3', 'p0', 'p5', 'p2', 'p7',  17,  49, 36, 39)
	TFBIG_MIX8  ('p6', 'p1', 'p0', 'p7', 'p2', 'p5', 'p4', 'p3',  44,   9, 54, 56)
    "#endregion"
}

function TFBIG_4o($s)
{
    "#region TFBIG_4o($s)"
    TFBIG_ADDKEY('p0', 'p1', 'p2', 'p3', 'p4', 'p5', 'p6', 'p7', 'h', 't', $s)
	TFBIG_MIX8  ('p0', 'p1', 'p2', 'p3', 'p4', 'p5', 'p6', 'p7',  39,  30, 34, 24)
	TFBIG_MIX8  ('p2', 'p1', 'p4', 'p7', 'p6', 'p5', 'p0', 'p3',  13,  50, 10, 17)
	TFBIG_MIX8  ('p4', 'p1', 'p6', 'p3', 'p0', 'p5', 'p2', 'p7',  25,  29, 39, 43)
	TFBIG_MIX8  ('p6', 'p1', 'p0', 'p7', 'p2', 'p5', 'p4', 'p3',   8,  35, 56, 22)
    "#endregion"
}

Clear-Host

'#region Main'
''
TFBIG_KINIT('h0', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'h7', 'h8', 't0', 't1', 't2')
TFBIG_4e(0)
TFBIG_4o(1)
TFBIG_4e(2)
TFBIG_4o(3)
TFBIG_4e(4)
TFBIG_4o(5)
TFBIG_4e(6)
TFBIG_4o(7)
TFBIG_4e(8)
TFBIG_4o(9)
TFBIG_4e(10)
TFBIG_4o(11)
TFBIG_4e(12)
TFBIG_4o(13)
TFBIG_4e(14)
TFBIG_4o(15)
TFBIG_4e(16)
TFBIG_4o(17)
TFBIG_ADDKEY('p0', 'p1', 'p2', 'p3', 'p4', 'p5', 'p6', 'p7', 'h', 't', 18)
''
'#endregion'
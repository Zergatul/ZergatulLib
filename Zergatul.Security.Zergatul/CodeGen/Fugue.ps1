function TIX4($params)
{
    $i = 0
    $q   = $params[$i++]
    $x00 = $params[$i++]
    $x01 = $params[$i++]
    $x04 = $params[$i++]
    $x07 = $params[$i++]
    $x08 = $params[$i++]
    $x22 = $params[$i++]
    $x24 = $params[$i++]
    $x27 = $params[$i++]
    $x30 = $params[$i++]

    "#region TIX4"
    "$x22 ^= $x00;"
    "$x00 = $q;"
    "$x08 ^= $x00;"
    "$x01 ^= $x24;"
    "$x04 ^= $x27;"
    "$x07 ^= $x30;"
    "#endregion"
}

function CMIX30($params)
{
    $i = 0
    $x00 = $params[$i++]
    $x01 = $params[$i++]
    $x02 = $params[$i++]
    $x04 = $params[$i++]
    $x05 = $params[$i++]
    $x06 = $params[$i++]
    $x15 = $params[$i++]
    $x16 = $params[$i++]
    $x17 = $params[$i++]

    "$x00 ^= $x04;"
	"$x01 ^= $x05;"
	"$x02 ^= $x06;"
	"$x15 ^= $x04;"
	"$x16 ^= $x05;"
	"$x17 ^= $x06;"
}

function CMIX36($params)
{
    $i = 0
    $x00 = $params[$i++]
    $x01 = $params[$i++]
    $x02 = $params[$i++]
    $x04 = $params[$i++]
    $x05 = $params[$i++]
    $x06 = $params[$i++]
    $x18 = $params[$i++]
    $x19 = $params[$i++]
    $x20 = $params[$i++]

    "#region CMIX36"
    "$x00 ^= $x04;"
    "$x01 ^= $x05;"
    "$x02 ^= $x06;"
    "$x18 ^= $x04;"
    "$x19 ^= $x05;"
    "$x20 ^= $x06;"
    "#endregion"
}

function SMIX($params)
{
    $i = 0
    $x0 = $params[$i++]
    $x1 = $params[$i++]
    $x2 = $params[$i++]
    $x3 = $params[$i++]

    "#region SMIX"
    "c0 = 0;"
	"c1 = 0;"
	"c2 = 0;"
	"c3 = 0;"
	"r0 = 0;"
	"r1 = 0;"
	"r2 = 0;"
	"r3 = 0;"
	"tmp = mixtab0[$x0 >> 24];"
	"c0 ^= tmp;"
	"tmp = mixtab1[($x0 >> 16) & 0xFF];"
	"c0 ^= tmp;"
	"r1 ^= tmp;"
	"tmp = mixtab2[($x0 >>  8) & 0xFF];"
	"c0 ^= tmp;"
	"r2 ^= tmp;"
	"tmp = mixtab3[$x0 & 0xFF];"
	"c0 ^= tmp;"
	"r3 ^= tmp;"
	"tmp = mixtab0[$x1 >> 24];"
	"c1 ^= tmp;"
	"r0 ^= tmp;"
	"tmp = mixtab1[($x1 >> 16) & 0xFF];"
	"c1 ^= tmp;"
	"tmp = mixtab2[($x1 >>  8) & 0xFF];"
	"c1 ^= tmp;"
	"r2 ^= tmp;"
	"tmp = mixtab3[$x1 & 0xFF];"
	"c1 ^= tmp;"
	"r3 ^= tmp;"
	"tmp = mixtab0[$x2 >> 24];"
	"c2 ^= tmp;"
	"r0 ^= tmp;"
	"tmp = mixtab1[($x2 >> 16) & 0xFF];"
	"c2 ^= tmp;"
	"r1 ^= tmp;"
	"tmp = mixtab2[($x2 >>  8) & 0xFF];"
	"c2 ^= tmp;"
	"tmp = mixtab3[$x2 & 0xFF];"
	"c2 ^= tmp;"
	"r3 ^= tmp;"
	"tmp = mixtab0[$x3 >> 24];"
	"c3 ^= tmp;"
	"r0 ^= tmp;"
	"tmp = mixtab1[($x3 >> 16) & 0xFF];"
	"c3 ^= tmp;"
	"r1 ^= tmp;"
	"tmp = mixtab2[($x3 >>  8) & 0xFF];"
	"c3 ^= tmp;"
	"r2 ^= tmp;"
	"tmp = mixtab3[$x3 & 0xFF];"
	"c3 ^= tmp;"
	"$x0 = ((c0 ^ r0) & 0xFF000000)"
	"	| ((c1 ^ r1) & 0x00FF0000)"
	"	| ((c2 ^ r2) & 0x0000FF00)"
	"	| ((c3 ^ r3) & 0x000000FF);"
	"$x1 = ((c1 ^ (r0 << 8)) & 0xFF000000)"
	"	| ((c2 ^ (r1 << 8)) & 0x00FF0000)"
	"	| ((c3 ^ (r2 << 8)) & 0x0000FF00)"
	"	| ((c0 ^ (r3 >> 24)) & 0x000000FF);"
	"$x2 = ((c2 ^ (r0 << 16)) & 0xFF000000)"
	"	| ((c3 ^ (r1 << 16)) & 0x00FF0000)"
	"	| ((c0 ^ (r2 >> 16)) & 0x0000FF00)"
	"	| ((c1 ^ (r3 >> 16)) & 0x000000FF);"
	"$x3 = ((c3 ^ (r0 << 24)) & 0xFF000000)"
	"	| ((c0 ^ (r1 >> 8)) & 0x00FF0000)"
	"	| ((c1 ^ (r2 >> 8)) & 0x0000FF00)"
	"	| ((c2 ^ (r3 >> 8)) & 0x000000FF);"
    "#endregion"
}

function ROR($params)
{
    $n = $params[0]
    $s = $params[1]

    "ArrayRotateRight(s, $n);"
}

Clear-Host

###
#"#region"
#TIX4('p', 's00', 's01', 's04', 's07', 's08', 's22', 's24', 's27', 's30')
#CMIX36('s33', 's34', 's35', 's01', 's02', 's03', 's15', 's16', 's17')
#SMIX('s33', 's34', 's35', 's00')
#CMIX36('s30', 's31', 's32', 's34', 's35', 's00', 's12', 's13', 's14')
#SMIX('s30', 's31', 's32', 's33')
#CMIX36('s27', 's28', 's29', 's31', 's32', 's33', 's09', 's10', 's11')
#SMIX('s27', 's28', 's29', 's30')
#CMIX36('s24', 's25', 's26', 's28', 's29', 's30', 's06', 's07', 's08')
#SMIX('s24', 's25', 's26', 's27')
#"#endregion"
###
#"#region"
#TIX4('p', 's24', 's25', 's28', 's31', 's32', 's10', 's12', 's15', 's18');
#CMIX36('s21', 's22', 's23', 's25', 's26', 's27', 's03', 's04', 's05');
#SMIX('s21', 's22', 's23', 's24');
#CMIX36('s18', 's19', 's20', 's22', 's23', 's24', 's00', 's01', 's02');
#SMIX('s18', 's19', 's20', 's21');
#CMIX36('s15', 's16', 's17', 's19', 's20', 's21', 's33', 's34', 's35');
#SMIX('s15', 's16', 's17', 's18');
#CMIX36('s12', 's13', 's14', 's16', 's17', 's18', 's30', 's31', 's32');
#SMIX('s12', 's13', 's14', 's15');
#"#endregion"
###
#"#region"
#TIX4('p', 's12', 's13', 's16', 's19', 's20', 's34', 's00', 's03', 's06');
#CMIX36('s09', 's10', 's11', 's13', 's14', 's15', 's27', 's28', 's29');
#SMIX('s09', 's10', 's11', 's12');
#CMIX36('s06', 's07', 's08', 's10', 's11', 's12', 's24', 's25', 's26');
#SMIX('s06', 's07', 's08', 's09');
#CMIX36('s03', 's04', 's05', 's07', 's08', 's09', 's21', 's22', 's23');
#SMIX('s03', 's04', 's05', 's06');
#CMIX36('s00', 's01', 's02', 's04', 's05', 's06', 's18', 's19', 's20');
#SMIX('s00', 's01', 's02', 's03');
#"#endregion"
###
"#region"
"for (int i = 0; i < 32; i++)"
"{"
ROR(3, 36)
CMIX36('s[0]', 's[1]', 's[2]', 's[4]', 's[5]', 's[6]', 's[18]', 's[19]', 's[20]')
SMIX('s[0]', 's[1]', 's[2]', 's[3]')
"}"
""
"for (int i = 0; i < 13; i++)"
"{"
	"s[4] ^= s[0];"
	"s[9] ^= s[0];"
	"s[18] ^= s[0];"
	"s[27] ^= s[0];"
	ROR(9, 36)
	SMIX('s[0]', 's[1]', 's[2]', 's[3]')
	"s[4] ^= s[0];"
	"s[10] ^= s[0];"
	"s[18] ^= s[0];"
	"s[27] ^= s[0];"
	ROR(9, 36)
	SMIX('s[0]', 's[1]', 's[2]', 's[3]')
	"s[4] ^= s[0];"
	"s[10] ^= s[0];"
	"s[19] ^= s[0];"
	"s[27] ^= s[0];"
	ROR(9, 36)
	SMIX('s[0]', 's[1]', 's[2]', 's[3]')
	"s[4] ^= s[0];"
	"s[10] ^= s[0];"
	"s[19] ^= s[0];"
	"s[28] ^= s[0];"
	ROR(8, 36)
	SMIX('s[0]', 's[1]', 's[2]', 's[3]')
"}"
""
"s[4] ^= s[0];"
"s[9] ^= s[0];"
"s[18] ^= s[0];"
"s[27] ^= s[0];"
"#endregion"
###
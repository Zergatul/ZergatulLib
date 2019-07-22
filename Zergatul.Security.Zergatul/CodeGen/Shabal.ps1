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

function PERM_ELT($params)
{
    $i = 0
    $xa0 = $params[$i++]
    $xa1 = $params[$i++]
    $xb0 = $params[$i++]
    $xb1 = $params[$i++]
    $xb2 = $params[$i++]
    $xb3 = $params[$i++]
    $xc  = $params[$i++]
    $xm  = $params[$i++]

    "$xa0 = (($xa0 ^ (RotateLeft($xa1, 15) * 5) ^ $xc) * 3) ^ $xb1 ^ ($xb2 & ~$xb3) ^ $xm;"
    "$xb0 = ~(RotateLeft($xb0, 1) ^ $xa0);"
}

function PERM_STEP_0
{
	PERM_ELT('a0', 'ab', 'b0', 'bd', 'b9', 'b6', 'c8', 'm0')
	PERM_ELT('a1', 'a0', 'b1', 'be', 'ba', 'b7', 'c7', 'm1')
	PERM_ELT('a2', 'a1', 'b2', 'bf', 'bb', 'b8', 'c6', 'm2')
	PERM_ELT('a3', 'a2', 'b3', 'b0', 'bc', 'b9', 'c5', 'm3')
	PERM_ELT('a4', 'a3', 'b4', 'b1', 'bd', 'ba', 'c4', 'm4')
	PERM_ELT('a5', 'a4', 'b5', 'b2', 'be', 'bb', 'c3', 'm5')
	PERM_ELT('a6', 'a5', 'b6', 'b3', 'bf', 'bc', 'c2', 'm6')
	PERM_ELT('a7', 'a6', 'b7', 'b4', 'b0', 'bd', 'c1', 'm7')
	PERM_ELT('a8', 'a7', 'b8', 'b5', 'b1', 'be', 'c0', 'm8')
	PERM_ELT('a9', 'a8', 'b9', 'b6', 'b2', 'bf', 'cf', 'm9')
	PERM_ELT('aa', 'a9', 'ba', 'b7', 'b3', 'b0', 'ce', 'ma')
	PERM_ELT('ab', 'aa', 'bb', 'b8', 'b4', 'b1', 'cd', 'mb')
	PERM_ELT('a0', 'ab', 'bc', 'b9', 'b5', 'b2', 'cc', 'mc')
	PERM_ELT('a1', 'a0', 'bd', 'ba', 'b6', 'b3', 'cb', 'md')
	PERM_ELT('a2', 'a1', 'be', 'bb', 'b7', 'b4', 'ca', 'me')
	PERM_ELT('a3', 'a2', 'bf', 'bc', 'b8', 'b5', 'c9', 'mf')
}

function PERM_STEP_1
{
	PERM_ELT('a4', 'a3', 'b0', 'bd', 'b9', 'b6', 'c8', 'm0')
	PERM_ELT('a5', 'a4', 'b1', 'be', 'ba', 'b7', 'c7', 'm1')
	PERM_ELT('a6', 'a5', 'b2', 'bf', 'bb', 'b8', 'c6', 'm2')
	PERM_ELT('a7', 'a6', 'b3', 'b0', 'bc', 'b9', 'c5', 'm3')
	PERM_ELT('a8', 'a7', 'b4', 'b1', 'bd', 'ba', 'c4', 'm4')
	PERM_ELT('a9', 'a8', 'b5', 'b2', 'be', 'bb', 'c3', 'm5')
	PERM_ELT('aa', 'a9', 'b6', 'b3', 'bf', 'bc', 'c2', 'm6')
	PERM_ELT('ab', 'aa', 'b7', 'b4', 'b0', 'bd', 'c1', 'm7')
	PERM_ELT('a0', 'ab', 'b8', 'b5', 'b1', 'be', 'c0', 'm8')
	PERM_ELT('a1', 'a0', 'b9', 'b6', 'b2', 'bf', 'cf', 'm9')
	PERM_ELT('a2', 'a1', 'ba', 'b7', 'b3', 'b0', 'ce', 'ma')
	PERM_ELT('a3', 'a2', 'bb', 'b8', 'b4', 'b1', 'cd', 'mb')
	PERM_ELT('a4', 'a3', 'bc', 'b9', 'b5', 'b2', 'cc', 'mc')
	PERM_ELT('a5', 'a4', 'bd', 'ba', 'b6', 'b3', 'cb', 'md')
	PERM_ELT('a6', 'a5', 'be', 'bb', 'b7', 'b4', 'ca', 'me')
	PERM_ELT('a7', 'a6', 'bf', 'bc', 'b8', 'b5', 'c9', 'mf')
}

function PERM_STEP_2
{
	PERM_ELT('a8', 'a7', 'b0', 'bd', 'b9', 'b6', 'c8', 'm0')
	PERM_ELT('a9', 'a8', 'b1', 'be', 'ba', 'b7', 'c7', 'm1')
	PERM_ELT('aa', 'a9', 'b2', 'bf', 'bb', 'b8', 'c6', 'm2')
	PERM_ELT('ab', 'aa', 'b3', 'b0', 'bc', 'b9', 'c5', 'm3')
	PERM_ELT('a0', 'ab', 'b4', 'b1', 'bd', 'ba', 'c4', 'm4')
	PERM_ELT('a1', 'a0', 'b5', 'b2', 'be', 'bb', 'c3', 'm5')
	PERM_ELT('a2', 'a1', 'b6', 'b3', 'bf', 'bc', 'c2', 'm6')
	PERM_ELT('a3', 'a2', 'b7', 'b4', 'b0', 'bd', 'c1', 'm7')
	PERM_ELT('a4', 'a3', 'b8', 'b5', 'b1', 'be', 'c0', 'm8')
	PERM_ELT('a5', 'a4', 'b9', 'b6', 'b2', 'bf', 'cf', 'm9')
	PERM_ELT('a6', 'a5', 'ba', 'b7', 'b3', 'b0', 'ce', 'ma')
	PERM_ELT('a7', 'a6', 'bb', 'b8', 'b4', 'b1', 'cd', 'mb')
	PERM_ELT('a8', 'a7', 'bc', 'b9', 'b5', 'b2', 'cc', 'mc')
	PERM_ELT('a9', 'a8', 'bd', 'ba', 'b6', 'b3', 'cb', 'md')
	PERM_ELT('aa', 'a9', 'be', 'bb', 'b7', 'b4', 'ca', 'me')
	PERM_ELT('ab', 'aa', 'bf', 'bc', 'b8', 'b5', 'c9', 'mf')
}

function APPLY_P
{
    for ($i = 0; $i -lt 16; $i++)
    {
        "b$(hex($i)) = RotateLeft(b$(hex($i)), 17);"
    }
    ""
    "#region Step 0"
    PERM_STEP_0
    "#endregion"
    ""
    "#region Step 1"
    PERM_STEP_1
    "#endregion"
    ""
    "#region Step 2"
    PERM_STEP_2
    "#endregion"
    ""
    "ab = ab + c6;"
    "aa = aa + c5;"
    "a9 = a9 + c4;"
    "a8 = a8 + c3;"
    "a7 = a7 + c2;"
    "a6 = a6 + c1;"
    "a5 = a5 + c0;"
    "a4 = a4 + cf;"
    "a3 = a3 + ce;"
    "a2 = a2 + cd;"
    "a1 = a1 + cc;"
    "a0 = a0 + cb;"
    "ab = ab + ca;"
    "aa = aa + c9;"
    "a9 = a9 + c8;"
    "a8 = a8 + c7;"
    "a7 = a7 + c6;"
    "a6 = a6 + c5;"
    "a5 = a5 + c4;"
    "a4 = a4 + c3;"
    "a3 = a3 + c2;"
    "a2 = a2 + c1;"
    "a1 = a1 + c0;"
    "a0 = a0 + cf;"
    "ab = ab + ce;"
    "aa = aa + cd;"
    "a9 = a9 + cc;"
    "a8 = a8 + cb;"
    "a7 = a7 + ca;"
    "a6 = a6 + c9;"
    "a5 = a5 + c8;"
    "a4 = a4 + c7;"
    "a3 = a3 + c6;"
    "a2 = a2 + c5;"
    "a1 = a1 + c4;"
    "a0 = a0 + c3;"
}

function SWAP_BC
{
    for ($i = 0; $i -lt 16; $i++)
    {
        $b = "b$(hex($i))"
        $c = "c$(hex($i))"
        "tt = $b;"
        "$b = $c;"
        "$c = tt;"
    }
}

Clear-Host

#"#region P"
#""
#APPLY_P
#""
#"#endregion"

SWAP_BC
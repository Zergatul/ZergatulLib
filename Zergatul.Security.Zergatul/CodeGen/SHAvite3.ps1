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

function KEY_EXPAND_ELT($params)
{
    $k0 = $params[0]
    $k1 = $params[1]
    $k2 = $params[2]
    $k3 = $params[3]

    AES_ROUND_NOKEY($k1, $k2, $k3, $k0)

    Write-Output "t0 = $k0;"
    Write-Output "$k0 = $k1;"
    Write-Output "$k1 = $k2;"
    Write-Output "$k2 = $k3;"
    Write-Output "$k3 = t0;"
}

function Gen
{
    Write-Output "#region Round 0"
    Write-Output ""

    Write-Output 'x0 = p4 ^ rk00;'
    Write-Output 'x1 = p5 ^ rk01;'
    Write-Output 'x2 = p6 ^ rk02;'
    Write-Output 'x3 = p7 ^ rk03;'

    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3')

    Write-Output 'x0 ^= rk04;'
	Write-Output 'x1 ^= rk05;'
	Write-Output 'x2 ^= rk06;'
	Write-Output 'x3 ^= rk07;'

    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3')
                             
	Write-Output 'x0 ^= rk08;'
	Write-Output 'x1 ^= rk09;'
	Write-Output 'x2 ^= rk0a;'
	Write-Output 'x3 ^= rk0b;'

    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3')

    Write-Output 'x0 ^= rk0c;'
	Write-Output 'x1 ^= rk0d;'
	Write-Output 'x2 ^= rk0e;'
	Write-Output 'x3 ^= rk0f;'

    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3')

    Write-Output 'p0 ^= x0;'
	Write-Output 'p1 ^= x1;'
	Write-Output 'p2 ^= x2;'
	Write-Output 'p3 ^= x3;'

    Write-Output 'x0 = pc ^ rk10;'
	Write-Output 'x1 = pd ^ rk11;'
	Write-Output 'x2 = pe ^ rk12;'
	Write-Output 'x3 = pf ^ rk13;'

    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3')

    Write-Output 'x0 ^= rk14;'
	Write-Output 'x1 ^= rk15;'
	Write-Output 'x2 ^= rk16;'
	Write-Output 'x3 ^= rk17;'

    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3')

	Write-Output 'x0 ^= rk18;'
	Write-Output 'x1 ^= rk19;'
	Write-Output 'x2 ^= rk1a;'
	Write-Output 'x3 ^= rk1b;'

    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3')

	Write-Output 'x0 ^= rk1c;'
	Write-Output 'x1 ^= rk1d;'
	Write-Output 'x2 ^= rk1e;'
	Write-Output 'x3 ^= rk1f;'

    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3')

    Write-Output 'p8 ^= x0;'
	Write-Output 'p9 ^= x1;'
	Write-Output 'pa ^= x2;'
	Write-Output 'pb ^= x3;'

    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""

    Write-Output "for (int r = 0; r < 3; r++)"
    Write-Output "{"
    Write-Output "#region Round 1, 5, 9"
    Write-Output ""

    KEY_EXPAND_ELT('rk00', 'rk01', 'rk02', 'rk03')

    Write-Output 'rk00 ^= rk1c;  '
	Write-Output 'rk01 ^= rk1d;  '
	Write-Output 'rk02 ^= rk1e;  '
	Write-Output 'rk03 ^= rk1f;  '
	Write-Output 'if (r == 0) {  '
	Write-Output '	rk00 ^= c0;  '
	Write-Output '	rk01 ^= c1;  '
	Write-Output '	rk02 ^= c2;  '
	Write-Output '	rk03 ^= ~c3; '
	Write-Output '}              '
	Write-Output 'x0 = p0 ^ rk00;'
	Write-Output 'x1 = p1 ^ rk01;'
	Write-Output 'x2 = p2 ^ rk02;'
	Write-Output 'x3 = p3 ^ rk03;'

	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');

	KEY_EXPAND_ELT('rk04', 'rk05', 'rk06', 'rk07');

	Write-Output 'rk04 ^= rk00; '
	Write-Output 'rk05 ^= rk01; '
	Write-Output 'rk06 ^= rk02; '
	Write-Output 'rk07 ^= rk03; '
	Write-Output 'if (r == 1) { '
	Write-Output '	rk04 ^= c3; '
	Write-Output '	rk05 ^= c2; '
	Write-Output '	rk06 ^= c1; '
	Write-Output '	rk07 ^= ~c0;'
	Write-Output '}             '
	Write-Output 'x0 ^= rk04;   '
	Write-Output 'x1 ^= rk05;   '
	Write-Output 'x2 ^= rk06;   '
	Write-Output 'x3 ^= rk07;   '

	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');

	KEY_EXPAND_ELT('rk08', 'rk09', 'rk0a', 'rk0b');

	Write-Output 'rk08 ^= rk04;'
	Write-Output 'rk09 ^= rk05;'
	Write-Output 'rk0a ^= rk06;'
	Write-Output 'rk0b ^= rk07;'
	Write-Output 'x0 ^= rk08;  '
	Write-Output 'x1 ^= rk09;  '
	Write-Output 'x2 ^= rk0a;  '
	Write-Output 'x3 ^= rk0b;  '

	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');

	KEY_EXPAND_ELT('rk0c', 'rk0d', 'rk0e', 'rk0f');

	Write-Output 'rk0c ^= rk08;'
	Write-Output 'rk0d ^= rk09;'
	Write-Output 'rk0e ^= rk0a;'
	Write-Output 'rk0f ^= rk0b;'
	Write-Output 'x0 ^= rk0c;  '
	Write-Output 'x1 ^= rk0d;  '
	Write-Output 'x2 ^= rk0e;  '
	Write-Output 'x3 ^= rk0f;  '

	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');

	Write-Output 'pc ^= x0;'
	Write-Output 'pd ^= x1;'
	Write-Output 'pe ^= x2;'
	Write-Output 'pf ^= x3;'

	KEY_EXPAND_ELT('rk10', 'rk11', 'rk12', 'rk13');

	Write-Output 'rk10 ^= rk0c;  '
	Write-Output 'rk11 ^= rk0d;  '
	Write-Output 'rk12 ^= rk0e;  '
	Write-Output 'rk13 ^= rk0f;  '
	Write-Output 'x0 = p8 ^ rk10;'
	Write-Output 'x1 = p9 ^ rk11;'
	Write-Output 'x2 = pa ^ rk12;'
	Write-Output 'x3 = pb ^ rk13;'

	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');

	KEY_EXPAND_ELT('rk14', 'rk15', 'rk16', 'rk17');

	Write-Output 'rk14 ^= rk10;'
	Write-Output 'rk15 ^= rk11;'
	Write-Output 'rk16 ^= rk12;'
	Write-Output 'rk17 ^= rk13;'
	Write-Output 'x0 ^= rk14;  '
	Write-Output 'x1 ^= rk15;  '
	Write-Output 'x2 ^= rk16;  '
	Write-Output 'x3 ^= rk17;  '

	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');

	KEY_EXPAND_ELT('rk18', 'rk19', 'rk1a', 'rk1b');

	Write-Output 'rk18 ^= rk14;'
	Write-Output 'rk19 ^= rk15;'
	Write-Output 'rk1a ^= rk16;'
	Write-Output 'rk1b ^= rk17;'
	Write-Output 'x0 ^= rk18;  '
	Write-Output 'x1 ^= rk19;  '
	Write-Output 'x2 ^= rk1a;  '
	Write-Output 'x3 ^= rk1b;  '

	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');

	KEY_EXPAND_ELT('rk1c', 'rk1d', 'rk1e', 'rk1f');

	Write-Output 'rk1c ^= rk18; '
	Write-Output 'rk1d ^= rk19; '
	Write-Output 'rk1e ^= rk1a; '
	Write-Output 'rk1f ^= rk1b; '
	Write-Output 'if (r == 2) { '
	Write-Output '	rk1c ^= c2; '
	Write-Output '	rk1d ^= c3; '
	Write-Output '	rk1e ^= c0; '
	Write-Output '	rk1f ^= ~c1;'
	Write-Output '}             '
	Write-Output 'x0 ^= rk1c;   '
	Write-Output 'x1 ^= rk1d;   '
	Write-Output 'x2 ^= rk1e;   '
	Write-Output 'x3 ^= rk1f;   '

	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');

	Write-Output 'p4 ^= x0;'
	Write-Output 'p5 ^= x1;'
	Write-Output 'p6 ^= x2;'
	Write-Output 'p7 ^= x3;'

    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""

    Write-Output "#region Round 2, 6, 10"
    Write-Output ""
    Write-Output 'rk00 ^= rk19;  '
    Write-Output 'x0 = pc ^ rk00;'
    Write-Output 'rk01 ^= rk1a;  '
    Write-Output 'x1 = pd ^ rk01;'
    Write-Output 'rk02 ^= rk1b;  '
    Write-Output 'x2 = pe ^ rk02;'
    Write-Output 'rk03 ^= rk1c;  '
    Write-Output 'x3 = pf ^ rk03;'
    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
    Write-Output 'rk04 ^= rk1d;'
    Write-Output 'x0 ^= rk04;  '
    Write-Output 'rk05 ^= rk1e;'
    Write-Output 'x1 ^= rk05;  '
    Write-Output 'rk06 ^= rk1f;'
    Write-Output 'x2 ^= rk06;  '
    Write-Output 'rk07 ^= rk00;'
    Write-Output 'x3 ^= rk07;  '
    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
    Write-Output 'rk08 ^= rk01;'
    Write-Output 'x0 ^= rk08;  '
    Write-Output 'rk09 ^= rk02;'
    Write-Output 'x1 ^= rk09;  '
    Write-Output 'rk0a ^= rk03;'
    Write-Output 'x2 ^= rk0a;  '
    Write-Output 'rk0b ^= rk04;'
    Write-Output 'x3 ^= rk0b;  '
    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
    Write-Output 'rk0c ^= rk05;'
    Write-Output 'x0 ^= rk0c;  '
    Write-Output 'rk0d ^= rk06;'
    Write-Output 'x1 ^= rk0d;  '
    Write-Output 'rk0e ^= rk07;'
    Write-Output 'x2 ^= rk0e;  '
    Write-Output 'rk0f ^= rk08;'
    Write-Output 'x3 ^= rk0f;  '
    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
    Write-Output 'p8 ^= x0;      '
    Write-Output 'p9 ^= x1;      '
    Write-Output 'pa ^= x2;      '
    Write-Output 'pb ^= x3;      '
    Write-Output 'rk10 ^= rk09;  '
    Write-Output 'x0 = p4 ^ rk10;'
    Write-Output 'rk11 ^= rk0a;  '
    Write-Output 'x1 = p5 ^ rk11;'
    Write-Output 'rk12 ^= rk0b;  '
    Write-Output 'x2 = p6 ^ rk12;'
    Write-Output 'rk13 ^= rk0c;  '
    Write-Output 'x3 = p7 ^ rk13;'
    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
    Write-Output 'rk14 ^= rk0d;'
    Write-Output 'x0 ^= rk14;  '
    Write-Output 'rk15 ^= rk0e;'
    Write-Output 'x1 ^= rk15;  '
    Write-Output 'rk16 ^= rk0f;'
    Write-Output 'x2 ^= rk16;  '
    Write-Output 'rk17 ^= rk10;'
    Write-Output 'x3 ^= rk17;  '
    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
    Write-Output 'rk18 ^= rk11;'
    Write-Output 'x0 ^= rk18;  '
    Write-Output 'rk19 ^= rk12;'
    Write-Output 'x1 ^= rk19;  '
    Write-Output 'rk1a ^= rk13;'
    Write-Output 'x2 ^= rk1a;  '
    Write-Output 'rk1b ^= rk14;'
    Write-Output 'x3 ^= rk1b;  '
    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
    Write-Output 'rk1c ^= rk15;'
    Write-Output 'x0 ^= rk1c;  '
    Write-Output 'rk1d ^= rk16;'
    Write-Output 'x1 ^= rk1d;  '
    Write-Output 'rk1e ^= rk17;'
    Write-Output 'x2 ^= rk1e;  '
    Write-Output 'rk1f ^= rk18;'
    Write-Output 'x3 ^= rk1f;  '
    AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
    Write-Output 'p0 ^= x0;'
    Write-Output 'p1 ^= x1;'
    Write-Output 'p2 ^= x2;'
    Write-Output 'p3 ^= x3;'
    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""

    Write-Output "#region Round 3, 7, 11"
    Write-Output ""
    KEY_EXPAND_ELT('rk00', 'rk01', 'rk02', 'rk03');
	Write-Output 'rk00 ^= rk1c;  '
	Write-Output 'rk01 ^= rk1d;  '
	Write-Output 'rk02 ^= rk1e;  '
	Write-Output 'rk03 ^= rk1f;  '
	Write-Output 'x0 = p8 ^ rk00;'
	Write-Output 'x1 = p9 ^ rk01;'
	Write-Output 'x2 = pa ^ rk02;'
	Write-Output 'x3 = pb ^ rk03;'
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk04', 'rk05', 'rk06', 'rk07');
	Write-Output 'rk04 ^= rk00;'
	Write-Output 'rk05 ^= rk01;'
	Write-Output 'rk06 ^= rk02;'
	Write-Output 'rk07 ^= rk03;'
	Write-Output 'x0 ^= rk04;  '
	Write-Output 'x1 ^= rk05;  '
	Write-Output 'x2 ^= rk06;  '
	Write-Output 'x3 ^= rk07;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk08', 'rk09', 'rk0a', 'rk0b');
	Write-Output 'rk08 ^= rk04;'
	Write-Output 'rk09 ^= rk05;'
	Write-Output 'rk0a ^= rk06;'
	Write-Output 'rk0b ^= rk07;'
	Write-Output 'x0 ^= rk08;  '
	Write-Output 'x1 ^= rk09;  '
	Write-Output 'x2 ^= rk0a;  '
	Write-Output 'x3 ^= rk0b;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk0c', 'rk0d', 'rk0e', 'rk0f');
	Write-Output 'rk0c ^= rk08;'
	Write-Output 'rk0d ^= rk09;'
	Write-Output 'rk0e ^= rk0a;'
	Write-Output 'rk0f ^= rk0b;'
	Write-Output 'x0 ^= rk0c;  '
	Write-Output 'x1 ^= rk0d;  '
	Write-Output 'x2 ^= rk0e;  '
	Write-Output 'x3 ^= rk0f;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'p4 ^= x0;'
	Write-Output 'p5 ^= x1;'
	Write-Output 'p6 ^= x2;'
	Write-Output 'p7 ^= x3;'
	KEY_EXPAND_ELT('rk10', 'rk11', 'rk12', 'rk13');
	Write-Output 'rk10 ^= rk0c;  '
	Write-Output 'rk11 ^= rk0d;  '
	Write-Output 'rk12 ^= rk0e;  '
	Write-Output 'rk13 ^= rk0f;  '
	Write-Output 'x0 = p0 ^ rk10;'
	Write-Output 'x1 = p1 ^ rk11;'
	Write-Output 'x2 = p2 ^ rk12;'
	Write-Output 'x3 = p3 ^ rk13;'
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk14', 'rk15', 'rk16', 'rk17');
	Write-Output 'rk14 ^= rk10;'
	Write-Output 'rk15 ^= rk11;'
	Write-Output 'rk16 ^= rk12;'
	Write-Output 'rk17 ^= rk13;'
	Write-Output 'x0 ^= rk14;  '
	Write-Output 'x1 ^= rk15;  '
	Write-Output 'x2 ^= rk16;  '
	Write-Output 'x3 ^= rk17;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk18', 'rk19', 'rk1a', 'rk1b');
	Write-Output 'rk18 ^= rk14;'
	Write-Output 'rk19 ^= rk15;'
	Write-Output 'rk1a ^= rk16;'
	Write-Output 'rk1b ^= rk17;'
	Write-Output 'x0 ^= rk18;  '
	Write-Output 'x1 ^= rk19;  '
	Write-Output 'x2 ^= rk1a;  '
	Write-Output 'x3 ^= rk1b;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk1c', 'rk1d', 'rk1e', 'rk1f');
	Write-Output 'rk1c ^= rk18;'
	Write-Output 'rk1d ^= rk19;'
	Write-Output 'rk1e ^= rk1a;'
	Write-Output 'rk1f ^= rk1b;'
	Write-Output 'x0 ^= rk1c;  '
	Write-Output 'x1 ^= rk1d;  '
	Write-Output 'x2 ^= rk1e;  '
	Write-Output 'x3 ^= rk1f;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'pc ^= x0;'
	Write-Output 'pd ^= x1;'
	Write-Output 'pe ^= x2;'
	Write-Output 'pf ^= x3;'
    Write-Output ""
    Write-Output "#endregion"

    Write-Output ""

    Write-Output "#region Round 4, 8, 12"
    Write-Output ""
    Write-Output 'rk00 ^= rk19;  '
	Write-Output 'x0 = p4 ^ rk00;'
	Write-Output 'rk01 ^= rk1a;  '
	Write-Output 'x1 = p5 ^ rk01;'
	Write-Output 'rk02 ^= rk1b;  '
	Write-Output 'x2 = p6 ^ rk02;'
	Write-Output 'rk03 ^= rk1c;  '
	Write-Output 'x3 = p7 ^ rk03;'
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'rk04 ^= rk1d;'
	Write-Output 'x0 ^= rk04;  '
	Write-Output 'rk05 ^= rk1e;'
	Write-Output 'x1 ^= rk05;  '
	Write-Output 'rk06 ^= rk1f;'
	Write-Output 'x2 ^= rk06;  '
	Write-Output 'rk07 ^= rk00;'
	Write-Output 'x3 ^= rk07;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'rk08 ^= rk01;'
	Write-Output 'x0 ^= rk08;  '
	Write-Output 'rk09 ^= rk02;'
	Write-Output 'x1 ^= rk09;  '
	Write-Output 'rk0a ^= rk03;'
	Write-Output 'x2 ^= rk0a;  '
	Write-Output 'rk0b ^= rk04;'
	Write-Output 'x3 ^= rk0b;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'rk0c ^= rk05;'
	Write-Output 'x0 ^= rk0c;  '
	Write-Output 'rk0d ^= rk06;'
	Write-Output 'x1 ^= rk0d;  '
	Write-Output 'rk0e ^= rk07;'
	Write-Output 'x2 ^= rk0e;  '
	Write-Output 'rk0f ^= rk08;'
	Write-Output 'x3 ^= rk0f;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'p0 ^= x0;      '
	Write-Output 'p1 ^= x1;      '
	Write-Output 'p2 ^= x2;      '
	Write-Output 'p3 ^= x3;      '
	Write-Output 'rk10 ^= rk09;  '
	Write-Output 'x0 = pc ^ rk10;'
	Write-Output 'rk11 ^= rk0a;  '
	Write-Output 'x1 = pd ^ rk11;'
	Write-Output 'rk12 ^= rk0b;  '
	Write-Output 'x2 = pe ^ rk12;'
	Write-Output 'rk13 ^= rk0c;  '
	Write-Output 'x3 = pf ^ rk13;'
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'rk14 ^= rk0d;'
	Write-Output 'x0 ^= rk14;  '
	Write-Output 'rk15 ^= rk0e;'
	Write-Output 'x1 ^= rk15;  '
	Write-Output 'rk16 ^= rk0f;'
	Write-Output 'x2 ^= rk16;  '
	Write-Output 'rk17 ^= rk10;'
	Write-Output 'x3 ^= rk17;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'rk18 ^= rk11;'
	Write-Output 'x0 ^= rk18;  '
	Write-Output 'rk19 ^= rk12;'
	Write-Output 'x1 ^= rk19;  '
	Write-Output 'rk1a ^= rk13;'
	Write-Output 'x2 ^= rk1a;  '
	Write-Output 'rk1b ^= rk14;'
	Write-Output 'x3 ^= rk1b;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'rk1c ^= rk15;'
	Write-Output 'x0 ^= rk1c;  '
	Write-Output 'rk1d ^= rk16;'
	Write-Output 'x1 ^= rk1d;  '
	Write-Output 'rk1e ^= rk17;'
	Write-Output 'x2 ^= rk1e;  '
	Write-Output 'rk1f ^= rk18;'
	Write-Output 'x3 ^= rk1f;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'p8 ^= x0;'
	Write-Output 'p9 ^= x1;'
	Write-Output 'pa ^= x2;'
	Write-Output 'pb ^= x3;'
    Write-Output ""
    Write-Output "#endregion"

    Write-Output "}"

    Write-Output ""

    Write-Output "#region Round 13"
    Write-Output ""
	KEY_EXPAND_ELT('rk00', 'rk01', 'rk02', 'rk03');
	Write-Output 'rk00 ^= rk1c;  '
	Write-Output 'rk01 ^= rk1d;  '
	Write-Output 'rk02 ^= rk1e;  '
	Write-Output 'rk03 ^= rk1f;  '
	Write-Output 'x0 = p0 ^ rk00;'
	Write-Output 'x1 = p1 ^ rk01;'
	Write-Output 'x2 = p2 ^ rk02;'
	Write-Output 'x3 = p3 ^ rk03;'
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk04', 'rk05', 'rk06', 'rk07');
	Write-Output 'rk04 ^= rk00;'
	Write-Output 'rk05 ^= rk01;'
	Write-Output 'rk06 ^= rk02;'
	Write-Output 'rk07 ^= rk03;'
	Write-Output 'x0 ^= rk04;  '
	Write-Output 'x1 ^= rk05;  '
	Write-Output 'x2 ^= rk06;  '
	Write-Output 'x3 ^= rk07;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk08', 'rk09', 'rk0a', 'rk0b');
	Write-Output 'rk08 ^= rk04;'
	Write-Output 'rk09 ^= rk05;'
	Write-Output 'rk0a ^= rk06;'
	Write-Output 'rk0b ^= rk07;'
	Write-Output 'x0 ^= rk08;  '
	Write-Output 'x1 ^= rk09;  '
	Write-Output 'x2 ^= rk0a;  '
	Write-Output 'x3 ^= rk0b;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk0c', 'rk0d', 'rk0e', 'rk0f');
	Write-Output 'rk0c ^= rk08;'
	Write-Output 'rk0d ^= rk09;'
	Write-Output 'rk0e ^= rk0a;'
	Write-Output 'rk0f ^= rk0b;'
	Write-Output 'x0 ^= rk0c;  '
	Write-Output 'x1 ^= rk0d;  '
	Write-Output 'x2 ^= rk0e;  '
	Write-Output 'x3 ^= rk0f;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'pc ^= x0;'
	Write-Output 'pd ^= x1;'
	Write-Output 'pe ^= x2;'
	Write-Output 'pf ^= x3;'
	KEY_EXPAND_ELT('rk10', 'rk11', 'rk12', 'rk13');
	Write-Output 'rk10 ^= rk0c;  '
	Write-Output 'rk11 ^= rk0d;  '
	Write-Output 'rk12 ^= rk0e;  '
	Write-Output 'rk13 ^= rk0f;  '
	Write-Output 'x0 = p8 ^ rk10;'
	Write-Output 'x1 = p9 ^ rk11;'
	Write-Output 'x2 = pa ^ rk12;'
	Write-Output 'x3 = pb ^ rk13;'
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk14', 'rk15', 'rk16', 'rk17');
	Write-Output 'rk14 ^= rk10;'
	Write-Output 'rk15 ^= rk11;'
	Write-Output 'rk16 ^= rk12;'
	Write-Output 'rk17 ^= rk13;'
	Write-Output 'x0 ^= rk14;  '
	Write-Output 'x1 ^= rk15;  '
	Write-Output 'x2 ^= rk16;  '
	Write-Output 'x3 ^= rk17;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk18', 'rk19', 'rk1a', 'rk1b');
	Write-Output 'rk18 ^= rk14 ^ c1;          '
	Write-Output 'rk19 ^= rk15 ^ c0;          '
	Write-Output 'rk1a ^= rk16 ^ c3;          '
	Write-Output 'rk1b ^= rk17 ^ ~c2;'
	Write-Output 'x0 ^= rk18;'
	Write-Output 'x1 ^= rk19;'
	Write-Output 'x2 ^= rk1a;'
	Write-Output 'x3 ^= rk1b;'
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	KEY_EXPAND_ELT('rk1c', 'rk1d', 'rk1e', 'rk1f');
	Write-Output 'rk1c ^= rk18;'
	Write-Output 'rk1d ^= rk19;'
	Write-Output 'rk1e ^= rk1a;'
	Write-Output 'rk1f ^= rk1b;'
	Write-Output 'x0 ^= rk1c;  '
	Write-Output 'x1 ^= rk1d;  '
	Write-Output 'x2 ^= rk1e;  '
	Write-Output 'x3 ^= rk1f;  '
	AES_ROUND_NOKEY('x0', 'x1', 'x2', 'x3');
	Write-Output 'p4 ^= x0;'
	Write-Output 'p5 ^= x1;'
	Write-Output 'p6 ^= x2;'
	Write-Output 'p7 ^= x3;'
    Write-Output ""
    Write-Output "#endregion"
}

clear

Gen
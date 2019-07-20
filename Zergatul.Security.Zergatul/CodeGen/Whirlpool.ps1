function BYTE($params)
{
    $x = $params[0]
    $n = $params[1]

    "($x >> $((8 * $n))) & 0xFF"
}

function ROUND_ELT($params)
{
    $i = 0
    $table = $params[$i++]
    $in    = $params[$i++]
    $i0    = $params[$i++]
    $i1    = $params[$i++]
    $i2    = $params[$i++]
    $i3    = $params[$i++]
    $i4    = $params[$i++]
    $i5    = $params[$i++]
    $i6    = $params[$i++]
    $i7    = $params[$i++]

    "$($table)0[$(BYTE(($in + $i0), 0))] ^ $($table)1[$(BYTE(($in + $i1), 1))] ^ $($table)2[$(BYTE(($in + $i2), 2))] ^ $($table)3[$(BYTE(($in + $i3), 3))] ^ $($table)4[$(BYTE(($in + $i4), 4))] ^ $($table)5[$(BYTE(($in + $i5), 5))] ^ $($table)6[$(BYTE(($in + $i6), 6))] ^ $($table)7[$(BYTE(($in + $i7), 7))]" 
}

function ROUND($params)
{
    $i = 0
    $table = $params[$i++]
    $in    = $params[$i++]
    $out   = $params[$i++]
    $c0    = $params[$i++]
    $c1    = $params[$i++]
    $c2    = $params[$i++]
    $c3    = $params[$i++]
    $c4    = $params[$i++]
    $c5    = $params[$i++]
    $c6    = $params[$i++]
    $c7    = $params[$i++]

    "$($out)0 = $(ROUND_ELT($table, $in, 0, 7, 6, 5, 4, 3, 2, 1)) ^ $c0;"
    "$($out)1 = $(ROUND_ELT($table, $in, 1, 0, 7, 6, 5, 4, 3, 2)) ^ $c1;"
    "$($out)2 = $(ROUND_ELT($table, $in, 2, 1, 0, 7, 6, 5, 4, 3)) ^ $c2;"
    "$($out)3 = $(ROUND_ELT($table, $in, 3, 2, 1, 0, 7, 6, 5, 4)) ^ $c3;"
    "$($out)4 = $(ROUND_ELT($table, $in, 4, 3, 2, 1, 0, 7, 6, 5)) ^ $c4;"
    "$($out)5 = $(ROUND_ELT($table, $in, 5, 4, 3, 2, 1, 0, 7, 6)) ^ $c5;"
    "$($out)6 = $(ROUND_ELT($table, $in, 6, 5, 4, 3, 2, 1, 0, 7)) ^ $c6;"
    "$($out)7 = $(ROUND_ELT($table, $in, 7, 6, 5, 4, 3, 2, 1, 0)) ^ $c7;"
}

function ROUND_KSCHED($params)
{
    $i = 0
    $table = $params[$i++]
    $in    = $params[$i++]
    $out   = $params[$i++]
    $c     = $params[$i++]

    ROUND($table, $in, $out, $c, 0, 0, 0, 0, 0, 0, 0)
}

function ROUND_WENC($params)
{
    $i = 0
    $table = $params[$i++]
    $in    = $params[$i++]
    $key   = $params[$i++]
    $out   = $params[$i++]

    ROUND($table, $in, $out, ($key + '0'), ($key + '1'), ($key + '2'), ($key + '3'), ($key + '4'), ($key + '5'), ($key + '6'), ($key + '7'))
}

Clear-Host
# ROUND_KSCHED('T', 'h', 't', 'RC[r]')
ROUND_WENC('T', 'n', 'h', 't')
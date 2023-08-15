//Maya ASCII 2016ff07 scene
//Name: hole.ma
//Last modified: Fri, May 12, 2017 02:08:28 PM
//Codeset: 1252
requires maya "2016ff07";
requires "stereoCamera" "10.0";
currentUnit -l centimeter -a degree -t film;
fileInfo "application" "maya";
fileInfo "product" "Maya 2016";
fileInfo "version" "2016";
fileInfo "cutIdentifier" "201603180400-990260-1";
fileInfo "osv" "Microsoft Windows 8 Business Edition, 64-bit  (Build 9200)\n";
fileInfo "license" "student";
createNode transform -n "pCylinder1";
	rename -uid "11013609-452E-50B6-F691-70B0B80B62D6";
	setAttr ".t" -type "double3" 0 -0.084422083491451458 0 ;
createNode transform -n "transform1" -p "pCylinder1";
	rename -uid "CD69B796-48F7-6AB0-FE3D-C5B8AB37F14E";
	setAttr ".v" no;
createNode mesh -n "pCylinderShape1" -p "transform1";
	rename -uid "01B164C4-49E0-B705-7B08-12980C8E64EF";
	setAttr -k off ".v";
	setAttr ".io" yes;
	setAttr -s 2 ".iog[0].og";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr -s 2 ".ciog[0].cog";
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
createNode transform -n "pCylinder2";
	rename -uid "E2C646EF-4AF0-58E8-6CB8-E087F460A4B1";
	setAttr ".t" -type "double3" 0 -0.10778605389931756 0 ;
	setAttr ".s" -type "double3" 1.0728358637645794 1.0728358637645794 1.0728358637645794 ;
createNode transform -n "transform2" -p "pCylinder2";
	rename -uid "23FCEB8A-4272-2253-3825-559CFECA1CA7";
	setAttr ".v" no;
createNode mesh -n "pCylinderShape2" -p "transform2";
	rename -uid "520BD4D9-4B79-6C24-354E-7983E1337771";
	setAttr -k off ".v";
	setAttr ".io" yes;
	setAttr -s 2 ".iog[0].og";
	setAttr ".iog[0].og[1].gcl" -type "componentList" 1 "f[0:59]";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr -s 2 ".ciog[0].cog";
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr -s 84 ".uvst[0].uvsp[0:83]" -type "float2" 0.64860266 0.10796607
		 0.62640899 0.064408496 0.59184152 0.029841021 0.54828393 0.0076473355 0.5 -7.4505806e-008
		 0.45171607 0.0076473504 0.40815851 0.029841051 0.37359107 0.064408526 0.3513974 0.10796608
		 0.34374997 0.15625 0.3513974 0.20453392 0.37359107 0.24809146 0.40815854 0.28265893
		 0.4517161 0.3048526 0.5 0.3125 0.54828387 0.3048526 0.59184146 0.28265893 0.62640893
		 0.24809146 0.6486026 0.2045339 0.65625 0.15625 0.375 0.3125 0.38749999 0.3125 0.39999998
		 0.3125 0.41249996 0.3125 0.42499995 0.3125 0.43749994 0.3125 0.44999993 0.3125 0.46249992
		 0.3125 0.4749999 0.3125 0.48749989 0.3125 0.49999988 0.3125 0.51249987 0.3125 0.52499986
		 0.3125 0.53749985 0.3125 0.54999983 0.3125 0.56249982 0.3125 0.57499981 0.3125 0.5874998
		 0.3125 0.59999979 0.3125 0.61249977 0.3125 0.62499976 0.3125 0.375 0.68843985 0.38749999
		 0.68843985 0.39999998 0.68843985 0.41249996 0.68843985 0.42499995 0.68843985 0.43749994
		 0.68843985 0.44999993 0.68843985 0.46249992 0.68843985 0.4749999 0.68843985 0.48749989
		 0.68843985 0.49999988 0.68843985 0.51249987 0.68843985 0.52499986 0.68843985 0.53749985
		 0.68843985 0.54999983 0.68843985 0.56249982 0.68843985 0.57499981 0.68843985 0.5874998
		 0.68843985 0.59999979 0.68843985 0.61249977 0.68843985 0.62499976 0.68843985 0.64860266
		 0.79546607 0.62640899 0.75190848 0.59184152 0.71734101 0.54828393 0.69514734 0.5
		 0.68749994 0.45171607 0.69514734 0.40815851 0.71734107 0.37359107 0.75190854 0.3513974
		 0.79546607 0.34374997 0.84375 0.3513974 0.89203393 0.37359107 0.93559146 0.40815854
		 0.97015893 0.4517161 0.9923526 0.5 1 0.54828387 0.9923526 0.59184146 0.97015893 0.62640893
		 0.93559146 0.6486026 0.89203393 0.65625 0.84375 0.5 0.15000001 0.5 0.83749998;
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
	setAttr -s 42 ".vt[0:41]"  0.10266662 -0.1 -0.033358406 0.087333448 -0.1 -0.063451454
		 0.063451454 -0.1 -0.087333441 0.033358403 -0.1 -0.10266661 0 -0.1 -0.10795005 -0.033358403 -0.1 -0.1026666
		 -0.063451447 -0.1 -0.087333418 -0.087333411 -0.1 -0.063451439 -0.10266658 -0.1 -0.033358391
		 -0.10795002 -0.1 0 -0.10266658 -0.1 0.033358391 -0.087333404 -0.1 0.063451432 -0.063451432 -0.1 0.087333404
		 -0.033358391 -0.1 0.10266657 -3.2171608e-009 -0.1 0.10795002 0.033358384 -0.1 0.10266656
		 0.063451417 -0.1 0.087333396 0.087333389 -0.1 0.063451424 0.10266656 -0.1 0.033358388
		 0.10795 -0.1 0 0.10266662 0.1 -0.033358406 0.087333448 0.1 -0.063451454 0.063451454 0.1 -0.087333441
		 0.033358403 0.1 -0.10266661 0 0.1 -0.10795005 -0.033358403 0.1 -0.1026666 -0.063451447 0.1 -0.087333418
		 -0.087333411 0.1 -0.063451439 -0.10266658 0.1 -0.033358391 -0.10795002 0.1 0 -0.10266658 0.1 0.033358391
		 -0.087333404 0.1 0.063451432 -0.063451432 0.1 0.087333404 -0.033358391 0.1 0.10266657
		 -3.2171608e-009 0.1 0.10795002 0.033358384 0.1 0.10266656 0.063451417 0.1 0.087333396
		 0.087333389 0.1 0.063451424 0.10266656 0.1 0.033358388 0.10795 0.1 0 0 -0.1 0 0 0.1 0;
	setAttr -s 100 ".ed[0:99]"  0 1 0 1 2 0 2 3 0 3 4 0 4 5 0 5 6 0 6 7 0
		 7 8 0 8 9 0 9 10 0 10 11 0 11 12 0 12 13 0 13 14 0 14 15 0 15 16 0 16 17 0 17 18 0
		 18 19 0 19 0 0 20 21 0 21 22 0 22 23 0 23 24 0 24 25 0 25 26 0 26 27 0 27 28 0 28 29 0
		 29 30 0 30 31 0 31 32 0 32 33 0 33 34 0 34 35 0 35 36 0 36 37 0 37 38 0 38 39 0 39 20 0
		 0 20 1 1 21 1 2 22 1 3 23 1 4 24 1 5 25 1 6 26 1 7 27 1 8 28 1 9 29 1 10 30 1 11 31 1
		 12 32 1 13 33 1 14 34 1 15 35 1 16 36 1 17 37 1 18 38 1 19 39 1 40 0 1 40 1 1 40 2 1
		 40 3 1 40 4 1 40 5 1 40 6 1 40 7 1 40 8 1 40 9 1 40 10 1 40 11 1 40 12 1 40 13 1
		 40 14 1 40 15 1 40 16 1 40 17 1 40 18 1 40 19 1 20 41 1 21 41 1 22 41 1 23 41 1 24 41 1
		 25 41 1 26 41 1 27 41 1 28 41 1 29 41 1 30 41 1 31 41 1 32 41 1 33 41 1 34 41 1 35 41 1
		 36 41 1 37 41 1 38 41 1 39 41 1;
	setAttr -s 60 -ch 200 ".fc[0:59]" -type "polyFaces" 
		f 4 0 41 -21 -41
		mu 0 4 20 21 42 41
		f 4 1 42 -22 -42
		mu 0 4 21 22 43 42
		f 4 2 43 -23 -43
		mu 0 4 22 23 44 43
		f 4 3 44 -24 -44
		mu 0 4 23 24 45 44
		f 4 4 45 -25 -45
		mu 0 4 24 25 46 45
		f 4 5 46 -26 -46
		mu 0 4 25 26 47 46
		f 4 6 47 -27 -47
		mu 0 4 26 27 48 47
		f 4 7 48 -28 -48
		mu 0 4 27 28 49 48
		f 4 8 49 -29 -49
		mu 0 4 28 29 50 49
		f 4 9 50 -30 -50
		mu 0 4 29 30 51 50
		f 4 10 51 -31 -51
		mu 0 4 30 31 52 51
		f 4 11 52 -32 -52
		mu 0 4 31 32 53 52
		f 4 12 53 -33 -53
		mu 0 4 32 33 54 53
		f 4 13 54 -34 -54
		mu 0 4 33 34 55 54
		f 4 14 55 -35 -55
		mu 0 4 34 35 56 55
		f 4 15 56 -36 -56
		mu 0 4 35 36 57 56
		f 4 16 57 -37 -57
		mu 0 4 36 37 58 57
		f 4 17 58 -38 -58
		mu 0 4 37 38 59 58
		f 4 18 59 -39 -59
		mu 0 4 38 39 60 59
		f 4 19 40 -40 -60
		mu 0 4 39 40 61 60
		f 3 -1 -61 61
		mu 0 3 1 0 82
		f 3 -2 -62 62
		mu 0 3 2 1 82
		f 3 -3 -63 63
		mu 0 3 3 2 82
		f 3 -4 -64 64
		mu 0 3 4 3 82
		f 3 -5 -65 65
		mu 0 3 5 4 82
		f 3 -6 -66 66
		mu 0 3 6 5 82
		f 3 -7 -67 67
		mu 0 3 7 6 82
		f 3 -8 -68 68
		mu 0 3 8 7 82
		f 3 -9 -69 69
		mu 0 3 9 8 82
		f 3 -10 -70 70
		mu 0 3 10 9 82
		f 3 -11 -71 71
		mu 0 3 11 10 82
		f 3 -12 -72 72
		mu 0 3 12 11 82
		f 3 -13 -73 73
		mu 0 3 13 12 82
		f 3 -14 -74 74
		mu 0 3 14 13 82
		f 3 -15 -75 75
		mu 0 3 15 14 82
		f 3 -16 -76 76
		mu 0 3 16 15 82
		f 3 -17 -77 77
		mu 0 3 17 16 82
		f 3 -18 -78 78
		mu 0 3 18 17 82
		f 3 -19 -79 79
		mu 0 3 19 18 82
		f 3 -20 -80 60
		mu 0 3 0 19 82
		f 3 20 81 -81
		mu 0 3 80 79 83
		f 3 21 82 -82
		mu 0 3 79 78 83
		f 3 22 83 -83
		mu 0 3 78 77 83
		f 3 23 84 -84
		mu 0 3 77 76 83
		f 3 24 85 -85
		mu 0 3 76 75 83
		f 3 25 86 -86
		mu 0 3 75 74 83
		f 3 26 87 -87
		mu 0 3 74 73 83
		f 3 27 88 -88
		mu 0 3 73 72 83
		f 3 28 89 -89
		mu 0 3 72 71 83
		f 3 29 90 -90
		mu 0 3 71 70 83
		f 3 30 91 -91
		mu 0 3 70 69 83
		f 3 31 92 -92
		mu 0 3 69 68 83
		f 3 32 93 -93
		mu 0 3 68 67 83
		f 3 33 94 -94
		mu 0 3 67 66 83
		f 3 34 95 -95
		mu 0 3 66 65 83
		f 3 35 96 -96
		mu 0 3 65 64 83
		f 3 36 97 -97
		mu 0 3 64 63 83
		f 3 37 98 -98
		mu 0 3 63 62 83
		f 3 38 99 -99
		mu 0 3 62 81 83
		f 3 39 80 -100
		mu 0 3 81 80 83;
	setAttr ".cd" -type "dataPolyComponent" Index_Data Edge 0 ;
	setAttr ".cvd" -type "dataPolyComponent" Index_Data Vertex 0 ;
	setAttr ".pd[0]" -type "dataPolyComponent" Index_Data UV 0 ;
	setAttr ".hfd" -type "dataPolyComponent" Index_Data Face 0 ;
createNode transform -n "pCylinder3";
	rename -uid "1AADB218-4CC7-9C4A-9353-0582F896E118";
	setAttr ".t" -type "double3" 0 0.26734336130586361 0 ;
	setAttr ".s" -type "double3" 1.0728358637645794 1.0728358637645794 1.0728358637645794 ;
createNode transform -n "transform3" -p "pCylinder3";
	rename -uid "44BF557C-45F1-B12D-4546-B1A00F5B15E3";
	setAttr ".v" no;
createNode mesh -n "pCylinderShape3" -p "transform3";
	rename -uid "C66A6062-45A4-1773-A5D4-EE88284D9D42";
	setAttr -k off ".v";
	setAttr ".io" yes;
	setAttr -s 2 ".iog[0].og";
	setAttr ".iog[0].og[1].gcl" -type "componentList" 1 "f[0:59]";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr -s 2 ".ciog[0].cog";
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr -s 84 ".uvst[0].uvsp[0:83]" -type "float2" 0.64860266 0.10796607
		 0.62640899 0.064408496 0.59184152 0.029841021 0.54828393 0.0076473355 0.5 -7.4505806e-008
		 0.45171607 0.0076473504 0.40815851 0.029841051 0.37359107 0.064408526 0.3513974 0.10796608
		 0.34374997 0.15625 0.3513974 0.20453392 0.37359107 0.24809146 0.40815854 0.28265893
		 0.4517161 0.3048526 0.5 0.3125 0.54828387 0.3048526 0.59184146 0.28265893 0.62640893
		 0.24809146 0.6486026 0.2045339 0.65625 0.15625 0.375 0.3125 0.38749999 0.3125 0.39999998
		 0.3125 0.41249996 0.3125 0.42499995 0.3125 0.43749994 0.3125 0.44999993 0.3125 0.46249992
		 0.3125 0.4749999 0.3125 0.48749989 0.3125 0.49999988 0.3125 0.51249987 0.3125 0.52499986
		 0.3125 0.53749985 0.3125 0.54999983 0.3125 0.56249982 0.3125 0.57499981 0.3125 0.5874998
		 0.3125 0.59999979 0.3125 0.61249977 0.3125 0.62499976 0.3125 0.375 0.68843985 0.38749999
		 0.68843985 0.39999998 0.68843985 0.41249996 0.68843985 0.42499995 0.68843985 0.43749994
		 0.68843985 0.44999993 0.68843985 0.46249992 0.68843985 0.4749999 0.68843985 0.48749989
		 0.68843985 0.49999988 0.68843985 0.51249987 0.68843985 0.52499986 0.68843985 0.53749985
		 0.68843985 0.54999983 0.68843985 0.56249982 0.68843985 0.57499981 0.68843985 0.5874998
		 0.68843985 0.59999979 0.68843985 0.61249977 0.68843985 0.62499976 0.68843985 0.64860266
		 0.79546607 0.62640899 0.75190848 0.59184152 0.71734101 0.54828393 0.69514734 0.5
		 0.68749994 0.45171607 0.69514734 0.40815851 0.71734107 0.37359107 0.75190854 0.3513974
		 0.79546607 0.34374997 0.84375 0.3513974 0.89203393 0.37359107 0.93559146 0.40815854
		 0.97015893 0.4517161 0.9923526 0.5 1 0.54828387 0.9923526 0.59184146 0.97015893 0.62640893
		 0.93559146 0.6486026 0.89203393 0.65625 0.84375 0.5 0.15000001 0.5 0.83749998;
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
	setAttr -s 42 ".vt[0:41]"  0.10266662 -0.1 -0.033358406 0.087333448 -0.1 -0.063451454
		 0.063451454 -0.1 -0.087333441 0.033358403 -0.1 -0.10266661 0 -0.1 -0.10795005 -0.033358403 -0.1 -0.1026666
		 -0.063451447 -0.1 -0.087333418 -0.087333411 -0.1 -0.063451439 -0.10266658 -0.1 -0.033358391
		 -0.10795002 -0.1 0 -0.10266658 -0.1 0.033358391 -0.087333404 -0.1 0.063451432 -0.063451432 -0.1 0.087333404
		 -0.033358391 -0.1 0.10266657 -3.2171608e-009 -0.1 0.10795002 0.033358384 -0.1 0.10266656
		 0.063451417 -0.1 0.087333396 0.087333389 -0.1 0.063451424 0.10266656 -0.1 0.033358388
		 0.10795 -0.1 0 0.10266662 0.1 -0.033358406 0.087333448 0.1 -0.063451454 0.063451454 0.1 -0.087333441
		 0.033358403 0.1 -0.10266661 0 0.1 -0.10795005 -0.033358403 0.1 -0.1026666 -0.063451447 0.1 -0.087333418
		 -0.087333411 0.1 -0.063451439 -0.10266658 0.1 -0.033358391 -0.10795002 0.1 0 -0.10266658 0.1 0.033358391
		 -0.087333404 0.1 0.063451432 -0.063451432 0.1 0.087333404 -0.033358391 0.1 0.10266657
		 -3.2171608e-009 0.1 0.10795002 0.033358384 0.1 0.10266656 0.063451417 0.1 0.087333396
		 0.087333389 0.1 0.063451424 0.10266656 0.1 0.033358388 0.10795 0.1 0 0 -0.1 0 0 0.1 0;
	setAttr -s 100 ".ed[0:99]"  0 1 0 1 2 0 2 3 0 3 4 0 4 5 0 5 6 0 6 7 0
		 7 8 0 8 9 0 9 10 0 10 11 0 11 12 0 12 13 0 13 14 0 14 15 0 15 16 0 16 17 0 17 18 0
		 18 19 0 19 0 0 20 21 0 21 22 0 22 23 0 23 24 0 24 25 0 25 26 0 26 27 0 27 28 0 28 29 0
		 29 30 0 30 31 0 31 32 0 32 33 0 33 34 0 34 35 0 35 36 0 36 37 0 37 38 0 38 39 0 39 20 0
		 0 20 1 1 21 1 2 22 1 3 23 1 4 24 1 5 25 1 6 26 1 7 27 1 8 28 1 9 29 1 10 30 1 11 31 1
		 12 32 1 13 33 1 14 34 1 15 35 1 16 36 1 17 37 1 18 38 1 19 39 1 40 0 1 40 1 1 40 2 1
		 40 3 1 40 4 1 40 5 1 40 6 1 40 7 1 40 8 1 40 9 1 40 10 1 40 11 1 40 12 1 40 13 1
		 40 14 1 40 15 1 40 16 1 40 17 1 40 18 1 40 19 1 20 41 1 21 41 1 22 41 1 23 41 1 24 41 1
		 25 41 1 26 41 1 27 41 1 28 41 1 29 41 1 30 41 1 31 41 1 32 41 1 33 41 1 34 41 1 35 41 1
		 36 41 1 37 41 1 38 41 1 39 41 1;
	setAttr -s 60 -ch 200 ".fc[0:59]" -type "polyFaces" 
		f 4 0 41 -21 -41
		mu 0 4 20 21 42 41
		f 4 1 42 -22 -42
		mu 0 4 21 22 43 42
		f 4 2 43 -23 -43
		mu 0 4 22 23 44 43
		f 4 3 44 -24 -44
		mu 0 4 23 24 45 44
		f 4 4 45 -25 -45
		mu 0 4 24 25 46 45
		f 4 5 46 -26 -46
		mu 0 4 25 26 47 46
		f 4 6 47 -27 -47
		mu 0 4 26 27 48 47
		f 4 7 48 -28 -48
		mu 0 4 27 28 49 48
		f 4 8 49 -29 -49
		mu 0 4 28 29 50 49
		f 4 9 50 -30 -50
		mu 0 4 29 30 51 50
		f 4 10 51 -31 -51
		mu 0 4 30 31 52 51
		f 4 11 52 -32 -52
		mu 0 4 31 32 53 52
		f 4 12 53 -33 -53
		mu 0 4 32 33 54 53
		f 4 13 54 -34 -54
		mu 0 4 33 34 55 54
		f 4 14 55 -35 -55
		mu 0 4 34 35 56 55
		f 4 15 56 -36 -56
		mu 0 4 35 36 57 56
		f 4 16 57 -37 -57
		mu 0 4 36 37 58 57
		f 4 17 58 -38 -58
		mu 0 4 37 38 59 58
		f 4 18 59 -39 -59
		mu 0 4 38 39 60 59
		f 4 19 40 -40 -60
		mu 0 4 39 40 61 60
		f 3 -1 -61 61
		mu 0 3 1 0 82
		f 3 -2 -62 62
		mu 0 3 2 1 82
		f 3 -3 -63 63
		mu 0 3 3 2 82
		f 3 -4 -64 64
		mu 0 3 4 3 82
		f 3 -5 -65 65
		mu 0 3 5 4 82
		f 3 -6 -66 66
		mu 0 3 6 5 82
		f 3 -7 -67 67
		mu 0 3 7 6 82
		f 3 -8 -68 68
		mu 0 3 8 7 82
		f 3 -9 -69 69
		mu 0 3 9 8 82
		f 3 -10 -70 70
		mu 0 3 10 9 82
		f 3 -11 -71 71
		mu 0 3 11 10 82
		f 3 -12 -72 72
		mu 0 3 12 11 82
		f 3 -13 -73 73
		mu 0 3 13 12 82
		f 3 -14 -74 74
		mu 0 3 14 13 82
		f 3 -15 -75 75
		mu 0 3 15 14 82
		f 3 -16 -76 76
		mu 0 3 16 15 82
		f 3 -17 -77 77
		mu 0 3 17 16 82
		f 3 -18 -78 78
		mu 0 3 18 17 82
		f 3 -19 -79 79
		mu 0 3 19 18 82
		f 3 -20 -80 60
		mu 0 3 0 19 82
		f 3 20 81 -81
		mu 0 3 80 79 83
		f 3 21 82 -82
		mu 0 3 79 78 83
		f 3 22 83 -83
		mu 0 3 78 77 83
		f 3 23 84 -84
		mu 0 3 77 76 83
		f 3 24 85 -85
		mu 0 3 76 75 83
		f 3 25 86 -86
		mu 0 3 75 74 83
		f 3 26 87 -87
		mu 0 3 74 73 83
		f 3 27 88 -88
		mu 0 3 73 72 83
		f 3 28 89 -89
		mu 0 3 72 71 83
		f 3 29 90 -90
		mu 0 3 71 70 83
		f 3 30 91 -91
		mu 0 3 70 69 83
		f 3 31 92 -92
		mu 0 3 69 68 83
		f 3 32 93 -93
		mu 0 3 68 67 83
		f 3 33 94 -94
		mu 0 3 67 66 83
		f 3 34 95 -95
		mu 0 3 66 65 83
		f 3 35 96 -96
		mu 0 3 65 64 83
		f 3 36 97 -97
		mu 0 3 64 63 83
		f 3 37 98 -98
		mu 0 3 63 62 83
		f 3 38 99 -99
		mu 0 3 62 81 83
		f 3 39 80 -100
		mu 0 3 81 80 83;
	setAttr ".cd" -type "dataPolyComponent" Index_Data Edge 0 ;
	setAttr ".cvd" -type "dataPolyComponent" Index_Data Vertex 0 ;
	setAttr ".pd[0]" -type "dataPolyComponent" Index_Data UV 0 ;
	setAttr ".hfd" -type "dataPolyComponent" Index_Data Face 0 ;
createNode transform -n "pCylinder4";
	rename -uid "7D644FD7-4DDB-8AD6-ADFB-E9B15B6293FA";
	setAttr ".rp" -type "double3" -1.4901161193847656e-008 -0.10778605565428734 -1.862645149230957e-008 ;
	setAttr ".sp" -type "double3" -1.4901161193847656e-008 -0.10778605565428734 -1.862645149230957e-008 ;
createNode transform -n "transform5" -p "pCylinder4";
	rename -uid "47172DA6-4C5F-1527-C814-AD8F333D77EA";
	setAttr ".v" no;
createNode mesh -n "pCylinder4Shape" -p "transform5";
	rename -uid "ABDF7950-470B-EA45-7FA4-9D8B3E420074";
	setAttr -k off ".v";
	setAttr ".io" yes;
	setAttr -s 2 ".iog[0].og";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr -s 4 ".ciog[0].cog";
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
createNode transform -n "pCylinder5";
	rename -uid "C2E61686-44BE-5029-AA03-099DAFE90EBC";
	setAttr ".t" -type "double3" 0 0.22930027793024627 0 ;
	setAttr ".s" -type "double3" 0.17973015001222642 0.0082517150404434435 0.17973015001222642 ;
createNode transform -n "transform4" -p "pCylinder5";
	rename -uid "7707BF94-4EF4-01FF-1BF6-04841B62D013";
	setAttr ".v" no;
createNode mesh -n "pCylinderShape4" -p "transform4";
	rename -uid "B3E431C7-4891-A1DA-D7B7-53803D9CC881";
	setAttr -k off ".v";
	setAttr ".io" yes;
	setAttr -s 2 ".iog[0].og";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
createNode transform -n "pCylinder7";
	rename -uid "BBF7ACD2-40B7-3C70-B2E6-B1BE9532C0A7";
	setAttr ".t" -type "double3" 0 -0.24142544229289448 0 ;
	setAttr ".rp" -type "double3" -2.2351741790771484e-008 0.22930027544498444 -3.7252902984619141e-008 ;
	setAttr ".sp" -type "double3" -2.2351741790771484e-008 0.22930027544498444 -3.7252902984619141e-008 ;
createNode transform -n "transform6" -p "pCylinder7";
	rename -uid "0A1223AF-4769-7CED-7E87-AEB9F4BDC48F";
	setAttr ".v" no;
createNode mesh -n "pCylinder7Shape" -p "transform6";
	rename -uid "1CBD4CCB-4A6B-8639-2044-4599BA57FFDA";
	setAttr -k off ".v";
	setAttr ".io" yes;
	setAttr -s 2 ".iog[0].og";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr -s 2 ".ciog[0].cog";
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
createNode transform -n "pCylinder8";
	rename -uid "C657FB9C-4EBB-2D79-ACC7-BCB6D919B5A4";
	setAttr ".rp" -type "double3" -2.2351741790771484e-008 -0.10778605565428734 -3.7252902984619141e-008 ;
	setAttr ".sp" -type "double3" -2.2351741790771484e-008 -0.10778605565428734 -3.7252902984619141e-008 ;
createNode mesh -n "pCylinder8Shape" -p "pCylinder8";
	rename -uid "56934274-4394-4C12-5355-149162B52318";
	setAttr -k off ".v";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
createNode polyCBoolOp -n "polyCBoolOp3";
	rename -uid "C38EF661-498C-46EC-07AC-239A78EF9C80";
	setAttr -s 2 ".ip";
	setAttr -s 2 ".im";
	setAttr ".mg" -type "Int32Array" 2 26 -28 ;
createNode groupParts -n "groupParts3";
	rename -uid "C09B7112-4820-BEF3-82CA-45815F467C89";
	setAttr ".ihi" 0;
	setAttr ".ic" -type "componentList" 1 "f[0:79]";
createNode polyCBoolOp -n "polyCBoolOp2";
	rename -uid "C487BA7F-4FA3-EA9F-A5BD-54AA6F54E878";
	setAttr -s 2 ".ip";
	setAttr -s 2 ".im";
	setAttr ".op" 2;
	setAttr ".mg" -type "Int32Array" 2 21 -23 ;
createNode groupId -n "groupId6";
	rename -uid "55789629-4012-8147-93AA-18B82615DF3A";
	setAttr ".ihi" 0;
createNode groupParts -n "groupParts2";
	rename -uid "4E495E69-4C17-9931-C858-EC9D1194DA67";
	setAttr ".ihi" 0;
	setAttr ".ic" -type "componentList" 1 "f[0:59]";
createNode polyCylinder -n "polyCylinder2";
	rename -uid "F5543236-4D91-D127-D398-8EBC6F90AB4D";
	setAttr ".sc" 1;
	setAttr ".cuv" 3;
createNode groupId -n "groupId7";
	rename -uid "ACD3E051-42B6-47F0-B5D0-9B8C89B4F32A";
	setAttr ".ihi" 0;
createNode groupId -n "groupId8";
	rename -uid "0136BC89-4F6D-CCAA-F798-5A94AA69F53E";
	setAttr ".ihi" 0;
createNode groupId -n "groupId9";
	rename -uid "6D4F5C5B-4361-3A6A-BECF-FF845EB53D2F";
	setAttr ".ihi" 0;
createNode groupId -n "groupId11";
	rename -uid "0A8B006D-4EE8-BF89-B308-718F24A27FB8";
	setAttr ".ihi" 0;
createNode groupId -n "groupId10";
	rename -uid "13AB37E7-4388-06D0-7A86-0688FF2D3CFB";
	setAttr ".ihi" 0;
createNode groupId -n "groupId12";
	rename -uid "942FD9CD-49A6-CC64-1DDB-41A61B3A11D0";
	setAttr ".ihi" 0;
createNode groupParts -n "groupParts4";
	rename -uid "3F4D28F7-4E54-6E03-B0CB-C49C80BCD12C";
	setAttr ".ihi" 0;
	setAttr ".ic" -type "componentList" 1 "f[0:99]";
createNode polyCBoolOp -n "polyCBoolOp1";
	rename -uid "F53F759B-40BF-4AEA-6783-E2937E23287D";
	setAttr -s 2 ".ip";
	setAttr -s 2 ".im";
	setAttr ".op" 2;
	setAttr ".mg" -type "Int32Array" 2 6 -8 ;
createNode groupId -n "groupId1";
	rename -uid "2E265A83-4911-642E-4322-9CAE98ABB1CC";
	setAttr ".ihi" 0;
createNode groupId -n "groupId2";
	rename -uid "FDA23DB3-4E38-C278-667E-659BB157441D";
	setAttr ".ihi" 0;
createNode groupId -n "groupId3";
	rename -uid "26DE979C-4BE8-29E1-70F7-7C98E9C2FE72";
	setAttr ".ihi" 0;
createNode groupParts -n "groupParts1";
	rename -uid "3CC3B5BB-48D3-08A7-0360-1796FE7BBE32";
	setAttr ".ihi" 0;
	setAttr ".ic" -type "componentList" 1 "f[0:59]";
createNode polyCylinder -n "polyCylinder1";
	rename -uid "FD9A23B1-415C-7C9B-5368-B7889C12FE54";
	setAttr ".r" 0.10795;
	setAttr ".h" 0.2;
	setAttr ".sc" 1;
	setAttr ".cuv" 3;
createNode groupId -n "groupId4";
	rename -uid "C5EE2BF0-4B7B-FB76-6AC0-909ADE253849";
	setAttr ".ihi" 0;
createNode groupId -n "groupId13";
	rename -uid "AC15A8A3-4EE1-3DCD-AE15-469A992B6566";
	setAttr ".ihi" 0;
createNode groupId -n "groupId5";
	rename -uid "9E8AD9E9-4D45-8116-4874-9EA8449EE569";
	setAttr ".ihi" 0;
createNode groupId -n "groupId14";
	rename -uid "9D58312E-4392-CADF-4C3E-8AB1479DC4D1";
	setAttr ".ihi" 0;
createNode groupId -n "groupId15";
	rename -uid "31858322-4B2A-85AA-8838-45AFD5F1F4DA";
	setAttr ".ihi" 0;
select -ne :time1;
	setAttr ".o" 1;
	setAttr ".unw" 1;
select -ne :hardwareRenderingGlobals;
	setAttr ".otfna" -type "stringArray" 22 "NURBS Curves" "NURBS Surfaces" "Polygons" "Subdiv Surface" "Particles" "Particle Instance" "Fluids" "Strokes" "Image Planes" "UI" "Lights" "Cameras" "Locators" "Joints" "IK Handles" "Deformers" "Motion Trails" "Components" "Hair Systems" "Follicles" "Misc. UI" "Ornaments"  ;
	setAttr ".otfva" -type "Int32Array" 22 0 1 1 1 1 1
		 1 1 1 0 0 0 0 0 0 0 0 0
		 0 0 0 0 ;
	setAttr ".fprt" yes;
select -ne :renderPartition;
	setAttr -s 2 ".st";
select -ne :renderGlobalsList1;
select -ne :defaultShaderList1;
	setAttr -s 4 ".s";
select -ne :postProcessList1;
	setAttr -s 2 ".p";
select -ne :defaultRenderingList1;
select -ne :initialShadingGroup;
	setAttr -s 17 ".dsm";
	setAttr ".ro" yes;
	setAttr -s 12 ".gn";
select -ne :initialParticleSE;
	setAttr ".ro" yes;
select -ne :defaultResolution;
	setAttr ".pa" 1;
select -ne :hardwareRenderGlobals;
	setAttr ".ctrs" 256;
	setAttr ".btrs" 512;
connectAttr "groupId3.id" "pCylinderShape1.iog.og[1].gid";
connectAttr ":initialShadingGroup.mwc" "pCylinderShape1.iog.og[1].gco";
connectAttr "groupParts1.og" "pCylinderShape1.i";
connectAttr "groupId4.id" "pCylinderShape1.ciog.cog[1].cgid";
connectAttr "groupId1.id" "pCylinderShape2.iog.og[1].gid";
connectAttr ":initialShadingGroup.mwc" "pCylinderShape2.iog.og[1].gco";
connectAttr "groupId2.id" "pCylinderShape2.ciog.cog[1].cgid";
connectAttr "groupId8.id" "pCylinderShape3.iog.og[1].gid";
connectAttr ":initialShadingGroup.mwc" "pCylinderShape3.iog.og[1].gco";
connectAttr "groupId9.id" "pCylinderShape3.ciog.cog[1].cgid";
connectAttr "groupParts4.og" "pCylinder4Shape.i";
connectAttr "groupId13.id" "pCylinder4Shape.iog.og[2].gid";
connectAttr ":initialShadingGroup.mwc" "pCylinder4Shape.iog.og[2].gco";
connectAttr "groupId5.id" "pCylinder4Shape.ciog.cog[0].cgid";
connectAttr "groupId14.id" "pCylinder4Shape.ciog.cog[3].cgid";
connectAttr "groupId6.id" "pCylinderShape4.iog.og[0].gid";
connectAttr ":initialShadingGroup.mwc" "pCylinderShape4.iog.og[0].gco";
connectAttr "groupParts2.og" "pCylinderShape4.i";
connectAttr "groupId7.id" "pCylinderShape4.ciog.cog[0].cgid";
connectAttr "groupParts3.og" "pCylinder7Shape.i";
connectAttr "groupId11.id" "pCylinder7Shape.iog.og[0].gid";
connectAttr ":initialShadingGroup.mwc" "pCylinder7Shape.iog.og[0].gco";
connectAttr "groupId10.id" "pCylinder7Shape.ciog.cog[0].cgid";
connectAttr "groupId12.id" "pCylinder7Shape.ciog.cog[1].cgid";
connectAttr "polyCBoolOp3.out" "pCylinder8Shape.i";
connectAttr "groupId15.id" "pCylinder8Shape.ciog.cog[0].cgid";
connectAttr "pCylinder7Shape.o" "polyCBoolOp3.ip[0]";
connectAttr "pCylinder4Shape.o" "polyCBoolOp3.ip[1]";
connectAttr "pCylinder7Shape.wm" "polyCBoolOp3.im[0]";
connectAttr "pCylinder4Shape.wm" "polyCBoolOp3.im[1]";
connectAttr "polyCBoolOp2.out" "groupParts3.ig";
connectAttr "groupId11.id" "groupParts3.gi";
connectAttr "pCylinderShape4.o" "polyCBoolOp2.ip[0]";
connectAttr "pCylinderShape3.o" "polyCBoolOp2.ip[1]";
connectAttr "pCylinderShape4.wm" "polyCBoolOp2.im[0]";
connectAttr "pCylinderShape3.wm" "polyCBoolOp2.im[1]";
connectAttr "polyCylinder2.out" "groupParts2.ig";
connectAttr "groupId6.id" "groupParts2.gi";
connectAttr "polyCBoolOp1.out" "groupParts4.ig";
connectAttr "groupId13.id" "groupParts4.gi";
connectAttr "pCylinderShape2.o" "polyCBoolOp1.ip[0]";
connectAttr "pCylinderShape1.o" "polyCBoolOp1.ip[1]";
connectAttr "pCylinderShape2.wm" "polyCBoolOp1.im[0]";
connectAttr "pCylinderShape1.wm" "polyCBoolOp1.im[1]";
connectAttr "polyCylinder1.out" "groupParts1.ig";
connectAttr "groupId3.id" "groupParts1.gi";
connectAttr "pCylinderShape2.iog.og[1]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinderShape2.ciog.cog[1]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinderShape1.iog.og[1]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinderShape1.ciog.cog[1]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinder4Shape.ciog.cog[0]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinderShape4.iog.og[0]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinderShape4.ciog.cog[0]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinderShape3.iog.og[1]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinderShape3.ciog.cog[1]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinder7Shape.ciog.cog[0]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinder7Shape.iog.og[0]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinder7Shape.ciog.cog[1]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinder4Shape.iog.og[2]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinder4Shape.ciog.cog[3]" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinder8Shape.iog" ":initialShadingGroup.dsm" -na;
connectAttr "pCylinder8Shape.ciog.cog[0]" ":initialShadingGroup.dsm" -na;
connectAttr "groupId1.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId2.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId3.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId4.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId6.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId7.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId8.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId9.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId11.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId12.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId13.msg" ":initialShadingGroup.gn" -na;
connectAttr "groupId14.msg" ":initialShadingGroup.gn" -na;
// End of hole.ma

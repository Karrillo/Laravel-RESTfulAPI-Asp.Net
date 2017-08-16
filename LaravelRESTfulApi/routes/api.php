<?php

use Illuminate\Http\Request;

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::middleware('auth:api')->get('/user', function (Request $request) {
    return $request->user();
});

Route::post('auth/register', 'UserController@register');
Route::post('auth/login', 'UserController@login');

Route::group(['middleware' => 'jwt.auth'], function () {
    Route::get('users', 'UserController@getAuthUser');
    Route::get('archives', 'ArchiveController@index');
    Route::post('archives', 'UserArchiveController@store');
    Route::get('/archives/{id}', 'UserArchiveController@show');
    Route::delete('archives/{id}', 'ArchiveController@destroy');
    Route::get('show/{id}', 'ArchiveController@showNormal');
});
